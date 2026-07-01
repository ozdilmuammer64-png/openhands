using UnityEngine;

namespace KnightOnline
{
    public class ItemPickup : MonoBehaviour
    {
        public ItemData itemData;
        public int quantity = 1;
        public float pickupRadius = 1f;
        public float rotateSpeed = 50f;
        public float bobSpeed = 2f;
        public float bobHeight = 0.3f;
        
        private Vector3 startPosition;
        private float time;
        private bool canPickup = true;
        
        public void Initialize(ItemData data, int qty = 1)
        {
            itemData = data;
            quantity = qty;
            startPosition = transform.position;
            
            // Set rarity color
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                switch (data.rarity)
                {
                    case ItemRarity.Common:
                        mat.color = Color.white;
                        break;
                    case ItemRarity.Uncommon:
                        mat.color = Color.green;
                        break;
                    case ItemRarity.Rare:
                        mat.color = new Color(0, 0.5f, 1f);
                        break;
                    case ItemRarity.Epic:
                        mat.color = new Color(0.6f, 0, 1f);
                        break;
                    case ItemRarity.Legendary:
                        mat.color = new Color(1f, 0.5f, 0);
                        break;
                }
                renderer.material = mat;
            }
            
            // Set name
            if (itemData.worldModel != null)
            {
                GameObject model = Instantiate(itemData.worldModel, transform);
                model.transform.localPosition = Vector3.up * 0.5f;
            }
            
            // Add glow effect for rare items
            if (data.rarity >= ItemRarity.Rare)
            {
                AddGlowEffect();
            }
        }
        
        void AddGlowEffect()
        {
            GameObject glow = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            glow.name = "Glow";
            glow.transform.parent = transform;
            glow.transform.localPosition = Vector3.zero;
            glow.transform.localScale = Vector3.one * 1.3f;
            
            Material glowMat = new Material(Shader.Find("Standard"));
            glowMat.color = new Color(1f, 1f, 0, 0.2f);
            glowMat.SetFloat("_Mode", 3);
            glow.GetComponent<Renderer>().material = glowMat;
            
            DestroyImmediate(glow.GetComponent<Collider>());
        }
        
        void Update()
        {
            if (!canPickup) return;
            
            // Rotate
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
            
            // Bob up and down
            time += Time.deltaTime;
            float yOffset = Mathf.Sin(time * bobSpeed) * bobHeight;
            transform.position = startPosition + Vector3.up * (0.5f + yOffset);
            
            // Check for player nearby
            CheckPlayerProximity();
        }
        
        void CheckPlayerProximity()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, pickupRadius, LayerMask.GetMask("Player"));
            if (hits.Length > 0)
            {
                PickupItem(hits[0].transform);
            }
        }
        
        void PickupItem(Transform player)
        {
            if (!canPickup) return;
            canPickup = false;
            
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Try to add to inventory
                bool added = InventorySystem.Instance?.AddItem(itemData, quantity) ?? false;
                
                if (added)
                {
                    UIManager.Instance?.ShowNotification($"+{quantity}x {itemData.itemName}", NotificationType.Loot);
                    
                    // Gold pickup sound or effect
                    if (itemData.itemType == ItemType.Material && itemData.goldBonusPercent > 0)
                    {
                        playerController.AddGold(quantity * 10);
                    }
                }
                else
                {
                    // Inventory full
                    UIManager.Instance?.ShowNotification("Envanter dolu!", NotificationType.Warning);
                    canPickup = true;
                    return;
                }
            }
            
            // Pickup animation
            StartCoroutine(PickupAnimation(player.position));
        }
        
        System.Collections.IEnumerator PickupAnimation(Vector3 targetPosition)
        {
            Vector3 startPos = transform.position;
            float duration = 0.2f;
            float elapsed = 0;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.position = Vector3.Lerp(startPos, targetPosition + Vector3.up, t);
                transform.localScale = Vector3.one * (1 - t);
                yield return null;
            }
            
            Destroy(gameObject);
        }
        
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, pickupRadius);
        }
    }
}
