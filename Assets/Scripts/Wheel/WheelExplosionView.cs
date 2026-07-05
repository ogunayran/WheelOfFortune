using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using WheelGame.Core;

namespace WheelGame.Wheel
{
    /// <summary>
    /// Purely cosmetic bomb feedback: a flash plus wheel shards and sparks flung
    /// outward from the center. Pieces are pooled once and reused every explosion.
    /// </summary>
    public class WheelExplosionView : MonoBehaviour
    {
        [SerializeField] private Sprite shardSprite;
        [SerializeField] private Sprite sparkSprite;
        [SerializeField] private Sprite flashSprite;
        [SerializeField, Min(1)] private int shardCount = 9;
        [SerializeField, Min(1)] private int sparkCount = 7;

        private readonly List<Image> pieces = new List<Image>();
        private Image flash;

        private void Awake()
        {
            flash = CreatePiece("ui_image_explosion_flash", flashSprite, 1020);
            flash.color = new Color(1f, 0.85f, 0.55f, 0f);

            for (int i = 0; i < shardCount; i++)
                pieces.Add(CreatePiece("ui_image_explosion_shard", shardSprite, Random.Range(64, 104)));
            for (int i = 0; i < sparkCount; i++)
            {
                Image spark = CreatePiece("ui_image_explosion_spark", sparkSprite, Random.Range(80, 150));
                spark.color = new Color(1f, 0.8f, 0.35f, 1f);
                pieces.Add(spark);
            }
        }

        private Image CreatePiece(string pieceName, Sprite sprite, float size)
        {
            var go = new GameObject(pieceName, typeof(RectTransform), typeof(Image));
            var rect = (RectTransform)go.transform;
            rect.SetParent(transform, false);
            rect.sizeDelta = new Vector2(size, size);

            var image = go.GetComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = true;
            image.raycastTarget = false;
            image.maskable = false;
            go.SetActive(false);
            return image;
        }

        private void OnEnable() => GameEvents.BombHit += Play;

        private void OnDisable()
        {
            GameEvents.BombHit -= Play;
            flash.DOKill();
            foreach (Image piece in pieces)
            {
                piece.DOKill();
                piece.rectTransform.DOKill();
            }
        }

        private void Play()
        {
            PlayFlash();
            foreach (Image piece in pieces)
                ThrowPiece(piece);
        }

        private void PlayFlash()
        {
            flash.DOKill();
            flash.gameObject.SetActive(true);
            flash.color = new Color(1f, 0.85f, 0.55f, 0.85f);
            flash.rectTransform.localScale = Vector3.one * 0.6f;
            flash.rectTransform.DOScale(1.15f, 0.28f).SetEase(Ease.OutCubic);
            flash.DOFade(0f, 0.3f).OnComplete(() => flash.gameObject.SetActive(false));
        }

        private void ThrowPiece(Image piece)
        {
            RectTransform rect = piece.rectTransform;
            rect.DOKill();
            piece.DOKill();

            piece.gameObject.SetActive(true);
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
            rect.localEulerAngles = Vector3.zero;
            Color color = piece.color;
            color.a = 1f;
            piece.color = color;

            Vector2 direction = Random.insideUnitCircle.normalized;
            float distance = Random.Range(280f, 620f);
            float duration = Random.Range(0.55f, 0.9f);
            float delay = Random.Range(0f, 0.08f);

            rect.DOAnchorPos(direction * distance, duration).SetDelay(delay).SetEase(Ease.OutCubic);
            rect.DORotate(new Vector3(0f, 0f, Random.Range(-900f, 900f)), duration, RotateMode.FastBeyond360)
                .SetDelay(delay);
            rect.DOScale(Random.Range(0.15f, 0.4f), duration).SetDelay(delay).SetEase(Ease.InQuad);
            piece.DOFade(0f, duration * 0.6f).SetDelay(delay + duration * 0.35f)
                 .OnComplete(() => piece.gameObject.SetActive(false));
        }
    }
}
