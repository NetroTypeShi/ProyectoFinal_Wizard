using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public float moveSpeed = 50f;
    public float fadeSpeed = 1f;

    private TextMeshProUGUI text;
    private Color color;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        color = text.color;
        color.a = 1f;
        text.color = color;
    }

    // ⭐ NUEVO: tipo de popup
    public void Setup(int amount, bool esCuracion)
    {
        if (esCuracion)
        {
            text.text = "+" + amount;
            text.color = Color.green;
        }
        else
        {
            text.text = "-" + amount;
            text.color = Color.red;
        }
    }

    private void Update()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        color = text.color;
        color.a -= fadeSpeed * Time.deltaTime;
        text.color = color;

        if (color.a <= 0)
            Destroy(gameObject);
    }
}



