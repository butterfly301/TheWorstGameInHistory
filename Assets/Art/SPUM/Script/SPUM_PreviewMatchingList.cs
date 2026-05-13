using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class SPUM_PreviewMatchingList : MonoBehaviour
{
    public List<PreviewMatchingElement> matchingTables = new();

    public void LoadItems()
    {
        matchingTables = new List<PreviewMatchingElement>();
        var Images = GetComponentsInChildren<Image>(true);
        var Text = "";
        foreach (var image in Images)
        {
            var item = new PreviewMatchingElement();
            item.Structure = Regex.Replace(image.name, @"[^a-zA-Z가-힣\s]", "");
            item.image = image;
            item.Color = Color.white;
            Text += item.Structure + "\n";
            matchingTables.Add(item);
        }

        Debug.Log(Text);
    }
}