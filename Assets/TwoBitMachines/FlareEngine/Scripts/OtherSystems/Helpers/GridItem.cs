using System;
using System.Collections.Generic;
using UnityEngine;
using Grid = TwoBitMachines.FlareEngine.AI.Grid;

namespace TwoBitMachines.FlareEngine
{
    public class GridItem : MonoBehaviour
    {
        [SerializeField] public bool gridElement = true;
        [SerializeField] public bool mustPay;
        [SerializeField] public bool save;
        [SerializeField] public float cost = 10f;
        [SerializeField] public string ID;

        [SerializeField] public string selectWE;
        [SerializeField] public string onFocusWE;
        [SerializeField] public string onOutOfFocusWE;
        [SerializeField] public string hasBeenUsedWE;
        [SerializeField] public string purchaseFailedWE;
        [SerializeField] public string cantPurchaseWE;
        [SerializeField] public string purchaseSuccessWE;
        [SerializeField] public string onHasBeenPurchasedWE;

        [SerializeField] public UnityEventEffect onSelect = new();
        [SerializeField] public UnityEventEffect onFocus = new();
        [SerializeField] public UnityEventEffect onOutOfFocus = new();
        [SerializeField] public UnityEventEffect purchaseFailed = new();
        [SerializeField] public UnityEventEffect purchaseSuccess = new();
        [SerializeField] public UnityEventEffect cantPurchase = new();
        [SerializeField] public UnityEventEffect onHasBeenPurchased = new();
        [NonSerialized] private readonly List<Transform> children = new();
        [NonSerialized] private Grid parent;

        [NonSerialized] public bool rememberPurchase;

        public void OnMouseDown()
        {
            if (parent != null) parent.SelectThisGridItem(transform);
        }

        public void Initialize(Grid parentRef)
        {
            parent = parentRef;
            WorldManager.get.worldResetAll -= ResetAll;
            WorldManager.get.worldResetAll += ResetAll;

            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                var gridItemChild = child.GetComponent<GridItem>();
                if (gridItemChild != null)
                {
                    gridItemChild.Initialize(parentRef);
                }
                else if (gridElement)
                {
                    child.SetParent(parentRef.transform.parent);
                    children.Add(child);
                    child.gameObject.SetActive(true);
                }
            }

            if (gridElement) gameObject.SetActive(true);
            if (save && parentRef.ItemIsPurchased(ID))
            {
                rememberPurchase = true;
                onHasBeenPurchased.Invoke(ImpactPacket.impact.Set(onHasBeenPurchasedWE, transform, null,
                    transform.position, null, Vector2.zero, 0, 0));
            }
        }

        public void ResetAll()
        {
            if (gameObject.activeInHierarchy)
                onOutOfFocus.Invoke(ImpactPacket.impact.Set(onOutOfFocusWE, transform, null, transform.position, null,
                    Vector2.zero, 0, 0));
            if (gridElement)
            {
                if (gameObject.activeInHierarchy) gameObject.SetActive(false);
                for (var i = 0; i < children.Count; i++)
                    if (children[i].gameObject.activeInHierarchy)
                    {
                        children[i].SetParent(transform);
                        children[i].gameObject.SetActive(false);
                    }
            }
        }

        public void OnSelect()
        {
            if (mustPay && rememberPurchase)
            {
                cantPurchase.Invoke(ImpactPacket.impact.Set(cantPurchaseWE, transform, null, transform.position, null,
                    Vector2.zero, 0, 0));
                return;
            }

            if (mustPay && parent != null && parent.payment != null)
            {
                var money = parent.payment.GetValue();
                if (money < cost || parent.payment.cantIncrement)
                {
                    purchaseFailed.Invoke(ImpactPacket.impact.Set(purchaseFailedWE, transform, null, transform.position,
                        null, Vector2.zero, 0, 0));
                    return;
                }

                if (parent.useTempValue)
                    parent.payment.IncreaseTempValue(-cost);
                else
                    parent.payment.Increment(-cost);
                if (save)
                {
                    parent.SaveItem(ID);
                    rememberPurchase = true;
                }

                purchaseSuccess.Invoke(ImpactPacket.impact.Set(purchaseSuccessWE, transform, null, transform.position,
                    null, Vector2.zero, 0, 0));
            }

            onSelect.Invoke(ImpactPacket.impact.Set(selectWE, transform, null, transform.position, null, Vector2.zero,
                0, 0));
        }

        public void OnFocus()
        {
            onFocus.Invoke(ImpactPacket.impact.Set(onFocusWE, transform, null, transform.position, null, Vector2.zero,
                0, 0));
        }

        public void OnOutOfFocus()
        {
            onOutOfFocus.Invoke(ImpactPacket.impact.Set(onOutOfFocusWE, transform, null, transform.position, null,
                Vector2.zero, 0, 0));
        }

        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] private bool eventsFoldOut;
        [SerializeField] [HideInInspector] private bool selectFoldOut;
        [SerializeField] [HideInInspector] private bool onFocusFoldOut;
        [SerializeField] [HideInInspector] private bool cantPurchaseFoldOut;
        [SerializeField] [HideInInspector] private bool onOutOfFocusFoldOut;
        [SerializeField] [HideInInspector] private bool purchaseFailedFoldOut;
        [SerializeField] [HideInInspector] private bool purchaseSuccessFoldOut;
        [SerializeField] [HideInInspector] private bool onHasBeenPurchasedFoldOut;
#pragma warning restore 0414
#endif

        #endregion
    }
}