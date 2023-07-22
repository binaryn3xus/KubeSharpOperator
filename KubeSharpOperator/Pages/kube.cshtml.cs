using k8s;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
                // Decode the base64 string and save it as a file at the base of the project
                byte[] kubeConfigBytes = Convert.FromBase64String(kubeConfigBase64);
                string kubeConfigFilePath = "kubeconfig.yaml"; // Change this to the desired file name and extension
                System.IO.File.WriteAllBytes(kubeConfigFilePath, kubeConfigBytes);

                // Build the Kubernetes client configuration from the file
                var config = KubernetesClientConfiguration.BuildConfigFromConfigFile(kubeConfigFilePath);
                var client = new Kubernetes(config);

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
