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
                var client = KubernetesClientExtensions.BuildConfigFromEnvVariable("KUBE_CONFIG");

                var namespaces = client.ListNamespace();
                foreach (var ns in namespaces.Items)
                {
                    var list = client.ListNamespacedPod(ns.Metadata.Name);
                    foreach (var item in list.Items)
                    {
                        itemList.Add($"{ns.Metadata.Name}:{item.Metadata.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return itemList;
        }

        private async Task CreateSimpleJobAsync(string namespaceName, string jobName, string containerImage)
        {
            var jobSpec = new V1Job
            {
                // Your Job specification, using the provided container image
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
