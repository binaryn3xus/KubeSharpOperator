using k8s;
using k8s.KubeConfigModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using YamlDotNet.Serialization;

namespace KubeSharpOperator.Pages
{
    public class kubeModel : PageModel
    {
        public void OnGet()
        {
            // Get the base64 string from the environment variable "KUBE_CONFIG"
            var kubeConfigBase64 = Environment.GetEnvironmentVariable("KUBE_CONFIG");
            if (string.IsNullOrEmpty(kubeConfigBase64))
            {
                Console.WriteLine("Error: KUBE_CONFIG environment variable not set.");
                return;
            }

            try
            {
                //// Decode the base64 string and save it as a file at the base of the project
                //byte[] kubeConfigBytes = Convert.FromBase64String(kubeConfigBase64);
                //string kubeConfigFilePath = "kubeconfig.yaml"; // Change this to the desired file name and extension
                //System.IO.File.WriteAllBytes(kubeConfigFilePath, kubeConfigBytes);

                //// Build the Kubernetes client configuration from the file
                //var config = KubernetesClientConfiguration.BuildConfigFromConfigFile(kubeConfigFilePath);
                //var client = new Kubernetes(config);

                // Decode the base64 string into bytes
                byte[] kubeConfigBytes = Convert.FromBase64String(kubeConfigBase64);

                // Convert the bytes to a UTF-8 encoded string (YAML content)
                string kubeConfigYaml = Encoding.UTF8.GetString(kubeConfigBytes);

                // Create an instance of the deserializer
                var deserializer = new DeserializerBuilder().Build();

                // Deserialize the kube config YAML into the K8SConfiguration object
                var k8sConfig = deserializer.Deserialize<K8SConfiguration>(kubeConfigYaml);
                var config = KubernetesClientConfiguration.BuildConfigFromConfigObject(k8sConfig);
                var client = new Kubernetes(config);

                // You can now work with the K8SConfiguration object
                // For example, access the current context name:
                string currentContext = k8sConfig.CurrentContext;
                Console.WriteLine($"Current Context: {currentContext}");

                var namespaces = client.ListNamespace();
                var itemList = new List<string>();
                foreach (var ns in namespaces.Items)
                {
                    Console.WriteLine($"Namespace: {ns.Metadata.Name}");
                    var list = client.ListNamespacedPod(ns.Metadata.Name);
                    foreach (var item in list.Items)
                    {
                        Console.WriteLine($"--- {ns.Metadata.Name}:{item.Metadata.Name}");
                        itemList.Add($"--- {ns.Metadata.Name}:{item.Metadata.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
