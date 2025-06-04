using UnityEngine;
using UnityEngine.UI;

public class HealthBarFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    private Camera cam;

    public Character character;
    public Slider slider;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Vector3 screenPos = cam.WorldToScreenPoint(target.position + offset);
        transform.position = screenPos;


    }
    public void UpdateHealthBar()
    {
        slider.maxValue = character.MaxHealth;
        slider.value = Mathf.Clamp(character.Health, 0, character.MaxHealth);
    }



}

