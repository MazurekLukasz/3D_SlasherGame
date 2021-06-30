using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectIndicator : MonoBehaviour
{
    public Canvas canvas;
    [SerializeField] private EnemyAI Target;
    public Image ImageHP;
    bool active;
    private void Start()
    {
        Target.OnHPChanged += (v) => {MaintainHpBar(v); };
        canvas.gameObject.SetActive(false);
    }
    float barShowingTime;
    private void MaintainHpBar(float value)
    {
        ImageHP.fillAmount = value;
        if (value <= 0)
        {
            canvas.gameObject.SetActive(false);
            active = false;
        }
        else
        {
            canvas.gameObject.SetActive(true);
            barShowingTime += 1f + Time.time;
            active = true;
        }
    }
    private void Update()
    {
        if (Time.time >= barShowingTime && active)
        {
            active = false;
            canvas.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        canvas.gameObject.transform.LookAt(Camera.main.transform);
        canvas.transform.Rotate(0,180,0);
    }
}
