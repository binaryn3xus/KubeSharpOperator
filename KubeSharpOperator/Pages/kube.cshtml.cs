using k8s;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KubeSharpOperator.Pages
{
    public class kubeModel : PageModel
    {
        public void OnGet()
        {
            try
            {
                var config = KubernetesClientConfiguration.InClusterConfig();
                var client = new Kubernetes(config);

                var namespaces = client.CoreV1.ListNamespace();
                var itemList = new List<string>();
                foreach (var ns in namespaces.Items)
                {
                    Console.WriteLine($"Namespace: {ns.Metadata.Name}");
                    var list = client.CoreV1.ListNamespacedPod(ns.Metadata.Name);
                    foreach (var item in list.Items)
                    {
                        Console.WriteLine(item.Metadata.Name);
                        itemList.Add($"{ns.Metadata.Name}:{item.Metadata.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
