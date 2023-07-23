using k8s;
using k8s.KubeConfigModels;
using KubeSharpOperator.Extensions;
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
            foreach(var podData in pods)
            {
                sb.AppendLine(podData);
            }
            ViewData["Pods"] = sb.ToString();
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
    }
}
