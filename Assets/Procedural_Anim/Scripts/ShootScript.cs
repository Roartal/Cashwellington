using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShootScript : MonoBehaviour
{
    [SerializeField]
    private Vector3 ShootOffset;

    [SerializeField]
    private GameObject ProjectilePrefab;

    [SerializeField]
    private Vector3 ShotDirection;

    [SerializeField]
    private float ShotForce;

    [SerializeField]
    private KeyCode keyCode;

    public Transform drum,gun,muzzle,gunParent;
    public Rigidbody player;
    public AnimationCurve animCurve;

    Vector3 initialPos;

    // Start is called before the first frame update
    void Start()
    {
        initialPos = gun.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        gun.localPosition = Vector3.Lerp(gun.localPosition, initialPos - player.velocity / 100f, Time.deltaTime * 5f);
        //Yeet "Projectile Prefab" with "ShotForce" in "ShotDirection" if "KeyCode" is pressed
        if (Input.GetKeyDown(keyCode))
        {
            var Bullet = GameObject.Instantiate(ProjectilePrefab, muzzle.position + muzzle.TransformDirection(ShootOffset), muzzle.rotation);
            transform.DOShakeRotation(0.3f, 0.4f, 10, 10f, true);
            Vector3 recoilVector = new Vector3(0f, 60f, 0f);
            drum.DOBlendableLocalRotateBy(recoilVector, 0.2f, RotateMode.Fast).SetLoops(1,LoopType.Yoyo).SetEase(Ease.InOutBounce);
            gun.localEulerAngles = new Vector3(0, 0, 0);
            gunParent.localEulerAngles = new Vector3(0, 0, 0);
            gun.DOLocalRotate(new Vector3(-50f, 0f, 0f), 0.4f, RotateMode.Fast).SetEase(animCurve);
            gunParent.DOShakeRotation(0.5f, 6f, 10, 90f, true);
        }
    }

    void HurtShake()
    {
        transform.DOShakeRotation(0.3f, 1f, 10, 90f, true);
        transform.DOShakePosition(0.3f, 0.1f, 1, 10f, false, true);
    }
}
