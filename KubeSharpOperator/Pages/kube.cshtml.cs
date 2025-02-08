using k8s;
using k8s.KubeConfigModels;
using k8s.Models;
using KubeSharpOperator.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace KubeSharpOperator.Pages
{
    public class kubeModel : PageModel
    {
        public void OnGet()
        {
            var pods = GetPods();
            StringBuilder sb = new StringBuilder();
            foreach (var podData in pods)
            {
                sb.AppendLine(podData);
            }
            ViewData["Pods"] = sb.ToString();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var namespaceName = "default"; // Change this to the desired namespace name
            var jobName = "testjob"; // Name of the Job (all lowercase)
            var containerImage = "ghcr.io/binaryn3xus/testjob:latest"; // The container image

            await DeleteExistingJobAsync(namespaceName, jobName);
            await CreateSimpleJobAsync(namespaceName, jobName, containerImage);

            return Content("Kubernetes Job triggered successfully!"); // Redirect back to the same page after running the Job
        }

        public List<string> GetPods()
        {
            var itemList = new List<string>();
            try
            {
                // Initialize Kubernetes client based on environment
                IKubernetes client;
                var kubeConfigEnvVar = Environment.GetEnvironmentVariable("KUBE_CONFIG");

                if (IsRunningInKubernetes())
                {
                    Console.WriteLine("Using InCluster configuration");
                    var config = KubernetesClientConfiguration.InClusterConfig();
                    client = new k8s.Kubernetes(config);
                }
                else if (!string.IsNullOrEmpty(kubeConfigEnvVar))
                {
                    Console.WriteLine("Using KUBE_CONFIG environment variable");
                    client = KubernetesClientExtensions.BuildConfigFromEnvVariable("KUBE_CONFIG");
                }
                else
                {
                    Console.WriteLine("Using default kubeconfig file location");
                    var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
                    client = new k8s.Kubernetes(config);
                }

                Console.WriteLine("Successfully created Kubernetes client");
                var namespaces = client.ListNamespace();
                Console.WriteLine($"Found {namespaces.Items.Count} namespaces");

                foreach (var ns in namespaces.Items)
                {
                    var list = client.ListNamespacedPod(ns.Metadata.Name);
                    Console.WriteLine($"Found {list.Items.Count} pods in namespace {ns.Metadata.Name}");

                    foreach (var item in list.Items)
                    {
                        itemList.Add($"{ns.Metadata.Name}:{item.Metadata.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPods: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                itemList.Add($"Error: {ex.Message}");
            }
            return itemList;
        }

        private bool IsRunningInKubernetes()
        {
            var isInK8s = File.Exists("/var/run/secrets/kubernetes.io/serviceaccount/token");
            Console.WriteLine($"IsRunningInKubernetes check: {isInK8s}");
            return isInK8s;
        }

        private async Task CreateSimpleJobAsync(string namespaceName, string jobName, string containerImage)
        {
            var jobSpec = new V1Job
            {
                Metadata = new V1ObjectMeta { Name = jobName },
                Spec = new V1JobSpec
                {
                    Completions = 1,
                    BackoffLimit = 2,
                    Parallelism = 1,
                    Template = new V1PodTemplateSpec
                    {
                        Spec = new V1PodSpec
                        {
                            Containers = new List<V1Container>
                            {
                                new V1Container
                                {
                                    Name = "testjob",
                                    Image = containerImage,
                                    Env = new List<V1EnvVar>
                                    {
                                        new V1EnvVar
                                        {
                                            Name = "HOSTNAME",
                                            Value = "dotnetevolved.com"
                                        },
                                        new V1EnvVar
                                        {
                                            Name = "LOOPCOUNT",
                                            Value = "5"
                                        },
                                        new V1EnvVar
                                        {
                                            Name = "SLEEPTIME",
                                            Value = "5000"
                                        }
                                    }
                                }
                            },
                            RestartPolicy = "Never",
                        }
                    }
                }
            };

            // Create the Kubernetes client and create the Job
            var client = KubernetesClientExtensions.BuildConfigFromEnvVariable("KUBE_CONFIG");
            await client.CreateNamespacedJobAsync(jobSpec, namespaceName);
        }

        private async Task DeleteExistingJobAsync(string namespaceName, string jobName)
        {
            var client = KubernetesClientExtensions.BuildConfigFromEnvVariable("KUBE_CONFIG");
            try
            {
                await client.DeleteNamespacedJobAsync(jobName, namespaceName, new V1DeleteOptions(), gracePeriodSeconds: 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
