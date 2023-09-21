using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace PlayerViewer.Handlers
{
    internal class PlayerHUD : MonoBehaviour
    {
        bool hasEnded = false;
        public TextMeshPro PlayerText;
        public string Username;
        BoxCollider2D boxCol;
        List<GameObject> collidedObjects = new List<GameObject>();


        public void Init(TextMeshPro textMesh, string name)
        {
            PlayerText = textMesh;
            Username = name;


            boxCol = gameObject.AddComponent<BoxCollider2D>();
            boxCol.offset += Vector2.down * 2f;
            boxCol.isTrigger = true;
            SetName(name);
        }


        void SetName(string name)
        {
            StartCoroutine(SetText(name));
        }

        IEnumerator SetText(string text)
        {
            PlayerText.text = text;
            boxCol.offset = PlayerText.transform.localPosition;
            yield return new WaitForSeconds(0.02f);
            boxCol.size = new Vector2(PlayerText.preferredWidth + 0.2f, 1);
        }



        void OnTriggerEnter2D(Collider2D col)
        {
            if (!collidedObjects.Contains(col.gameObject))
                collidedObjects.Add(col.gameObject);

            StopAllCoroutines();
            StartCoroutine(FadeText(PlayerText, 0.6f, 0.5f));
            
        }

        void OnTriggerExit2D(Collider2D col)
        {
            if (collidedObjects.Contains(col.gameObject))
                collidedObjects.Remove(col.gameObject); 
            
            if(collidedObjects.Count < 1)
            {
                StopAllCoroutines();
                StartCoroutine(FadeText(PlayerText, 1f, 0.5f));
            }
        }


        public void Finish()
        {
            StopAllCoroutines();
            StartCoroutine(RemoveObject());
            hasEnded = true;
        }

        private IEnumerator RemoveObject()
        {
            yield return FadeText(PlayerText, 0, 0.5f);
            Destroy(this);
        }


        private IEnumerator FadeText(TextMeshPro text, float targetAlpha, float fadeTime)
        {
            if (hasEnded) yield break;

            float elapsedTime = 0.0f;
            var initialColor = text.color;
            while (elapsedTime < fadeTime)
            {
                float alpha = Mathf.Lerp(initialColor.a, targetAlpha, elapsedTime / fadeTime);
                Color newColor = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
                text.color = newColor;
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            text.color = new Color(initialColor.r, initialColor.g, initialColor.b, targetAlpha);
        }

    }
}
