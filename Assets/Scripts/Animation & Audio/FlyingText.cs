using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlyingText : MonoBehaviour
{
    public TextMeshPro text;
    private Vector3 desiredDir;
    private Vector3 desiredScale;
    private float posSpeed = 0.12f;
    private float scaleSpeed;
    private Vector3 scaleVel;

    // Start is called before the first frame update
    void Awake()
    {
        desiredDir = Vector3.up * Random.Range(1.5f, 3f) + Vector3.right * Random.Range(-1.5f, 1.5f);
        desiredScale = base.transform.localScale;
        transform.localScale = Vector3.zero;
        Invoke("DestroySelf", 2f);
    }

    private void Start()
    {
        text.CrossFadeAlpha(0.01f, 1.5f, false);
    }

    public void SetText(string data = "999")
    {
        text.text = data;
    }

    Vector3 target;
    private void Update()
    {
        transform.position += this.desiredDir * this.posSpeed;
        posSpeed = Mathf.Lerp(this.posSpeed, 0.002f, Time.deltaTime * 13f);
        transform.localScale = Vector3.SmoothDamp(base.transform.localScale, this.desiredScale, ref this.scaleVel, 0.05f);

        target = transform.position + (transform.position - Camera.main.transform.position);

        Vector3 targetPostition = new Vector3(target.x,
                                       this.transform.position.y,
                                       target.z);
        this.transform.LookAt(targetPostition);
    }

    private void DestroySelf()
    {
        Destroy(transform.root.gameObject);
    }
}
