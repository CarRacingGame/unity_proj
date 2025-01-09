using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrometeoCarController : MonoBehaviour
{
    // --------------------------------------------------------------------------------------
    // 1) �ڵ��� ����(CAR SETUP) ���� ������
    // --------------------------------------------------------------------------------------
    [Space(20)]
    //[Header("CAR SETUP")]
    [Space(10)]
    [Range(20, 190)]
    public int maxSpeed = 90;
    // �ڵ����� ������ �� �ִ� �ִ� �ӵ�(km/h).

    [Range(10, 120)]
    public int maxReverseSpeed = 45;
    // �ڵ����� ���� �� ������ �� �ִ� �ִ� �ӵ�(km/h).

    [Range(1, 10)]
    public int accelerationMultiplier = 2;
    // ���ӵ� ���(���� �������� ������ ����).

    [Space(10)]
    [Range(10, 45)]
    public int maxSteeringAngle = 27;
    // ������(�ڵ�)�� ���� �� ������ ȸ���� �� �ִ� �ִ� ����.

    [Range(0.1f, 1f)]
    public float steeringSpeed = 0.5f;
    // ������ ȸ���ϴ� ���ǵ�(�ڵ� ���� �ӵ�).

    [Space(10)]
    [Range(100, 600)]
    public int brakeForce = 350;
    // ������ ����Ǵ� �극��ũ ���� ũ��.

    [Range(1, 10)]
    public int decelerationMultiplier = 2;
    // ������ ������ �� ������ �󸶳� ������ �����ϴ��� ����(���� �������� ���� ����).

    [Range(1, 10)]
    public int handbrakeDriftMultiplier = 5;
    // ���̵�극��ũ(�ڵ�극��ũ) ��� ��, ������ �󸶳� ���� �̲����������� ����(���� �������� �� ũ�� �̲�����).

    [Space(10)]
    public Vector3 bodyMassCenter;
    /*
     * ������ ���� �߽��� ��Ÿ���� ����.
     * �Ϲ������� x = 0, z = 0�� �ΰ�, y�ุ ������ �����մϴ�.
     * y���� ���������� ������ �� �Ҿ��������ϴ�.
    */

    // --------------------------------------------------------------------------------------
    // 2) ����(WHEELS) ����
    // --------------------------------------------------------------------------------------
    //[Header("WHEELS")]
    public GameObject frontLeftMesh;
    public WheelCollider frontLeftCollider;
    [Space(10)]
    public GameObject frontRightMesh;
    public WheelCollider frontRightCollider;
    [Space(10)]
    public GameObject rearLeftMesh;
    public WheelCollider rearLeftCollider;
    [Space(10)]
    public GameObject rearRightMesh;
    public WheelCollider rearRightCollider;
    /*
     * �� WheelCollider�� 3D �޽��� �и��� ���� ������Ʈ���� �մϴ�.
     * ��) WheelCollider ������Ʈ��, �׿� �ش��ϴ� 3D �� ��(�޽�)�� �ٸ� ���ӿ�����Ʈ.
    */

    // --------------------------------------------------------------------------------------
    // 3) ��ƼŬ ����Ʈ(PARTICLE SYSTEMS) ��� ���� & ����
    // --------------------------------------------------------------------------------------
    [Space(20)]
    //[Header("EFFECTS")]
    [Space(10)]
    public bool useEffects = false;
    // ��ƼŬ �� Ʈ���� ��� ����

    // ������ �帮��Ʈ�� �� ���Ǵ� ��ƼŬ �ý���(Ÿ�̾� ����ũ ��)
    public ParticleSystem RLWParticleSystem;
    public ParticleSystem RRWParticleSystem;

    [Space(10)]
    // ������ �̲����� �� ���Ǵ� Ʈ���� ������(��Ű�� ��ũ)
    public TrailRenderer RLWTireSkid;
    public TrailRenderer RRWTireSkid;

    // --------------------------------------------------------------------------------------
    // 4) UI - �ӵ� ǥ��(SPEED TEXT) ����
    // --------------------------------------------------------------------------------------
    [Space(20)]
    //[Header("UI")]
    [Space(10)]
    public bool useUI = false;
    // UI�� ������� ����
    public Text carSpeedText;
    // ���� �ӵ��� ǥ���� UI �ؽ�Ʈ

    // --------------------------------------------------------------------------------------
    // 5) ����(SOUNDS) ����
    // --------------------------------------------------------------------------------------
    [Space(20)]
    //[Header("Sounds")]
    [Space(10)]
    public bool useSounds = false;
    // ���� ���� ��� ����
    public AudioSource carEngineSound;
    // ���� ����
    public AudioSource tireScreechSound;
    // Ÿ�̾� ��Ű�� ����(�帮��Ʈ ��)
    float initialCarEngineSoundPitch;
    // ���� ���� �⺻ ��ġ ���� �����ϱ� ����

    // --------------------------------------------------------------------------------------
    // 6) ��Ʈ��(CONTROLS) - ����Ͽ� ��ġ ��Ʈ�� ����
    // --------------------------------------------------------------------------------------
    [Space(20)]
    //[Header("CONTROLS")]
    [Space(10)]
    public bool useTouchControls = false;
    // ��ġ ��Ʈ�� ��� ����

    public GameObject throttleButton;
    PrometeoTouchInput throttlePTI;
    public GameObject reverseButton;
    PrometeoTouchInput reversePTI;
    public GameObject turnRightButton;
    PrometeoTouchInput turnRightPTI;
    public GameObject turnLeftButton;
    PrometeoTouchInput turnLeftPTI;
    public GameObject handbrakeButton;
    PrometeoTouchInput handbrakePTI;

    // --------------------------------------------------------------------------------------
    // 7) ���� ����(CAR DATA)
    // --------------------------------------------------------------------------------------
    [HideInInspector]
    public float carSpeed;
    // ���� ���� �ӵ�(��� �뵵)

    [HideInInspector]
    public bool isDrifting;
    // ������ �帮��Ʈ ������ ����

    [HideInInspector]
    public bool isTractionLocked;
    // �ڵ�극��ũ�� ���� �������� ����(Ÿ�̾ ���) �������� ����

    // --------------------------------------------------------------------------------------
    // 8) ���ο����� ����ϴ� ������(PRIVATE VARIABLES)
    // --------------------------------------------------------------------------------------
    Rigidbody carRigidbody;
    // ������ Rigidbody
    float steeringAxis;
    // ��Ƽ�(�ڵ�) ��(-1 ~ 1)
    float throttleAxis;
    // ���� ���� ��(-1 ~ 1)
    float driftingAxis;
    float localVelocityZ;
    float localVelocityX;
    bool deceleratingCar;
    bool touchControlsSetup = false;

    // ������ ���� ���� Ŀ��. �帮��Ʈ�� Ʈ���ǿ� ���
    WheelFrictionCurve FLwheelFriction;
    float FLWextremumSlip;
    WheelFrictionCurve FRwheelFriction;
    float FRWextremumSlip;
    WheelFrictionCurve RLwheelFriction;
    float RLWextremumSlip;
    WheelFrictionCurve RRwheelFriction;
    float RRWextremumSlip;

    // --------------------------------------------------------------------------------------
    // Start() �޼���
    // ���� ���� �� ù �����ӿ� �� ���� ȣ��Ǹ�, ��ü ������ �ʱ�ȭ/����
    // --------------------------------------------------------------------------------------
    void Start()
    {
        // (1) Rigidbody�� �����ͼ�, �ν����Ϳ��� ������ ���� �߽����� ����
        carRigidbody = gameObject.GetComponent<Rigidbody>();
        carRigidbody.centerOfMass = bodyMassCenter;

        // (2) �帮��Ʈ�� ���� ��ġ �ʱ�ȭ
        FLwheelFriction = new WheelFrictionCurve();
        FLwheelFriction.extremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
        FLWextremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
        FLwheelFriction.extremumValue = frontLeftCollider.sidewaysFriction.extremumValue;
        FLwheelFriction.asymptoteSlip = frontLeftCollider.sidewaysFriction.asymptoteSlip;
        FLwheelFriction.asymptoteValue = frontLeftCollider.sidewaysFriction.asymptoteValue;
        FLwheelFriction.stiffness = frontLeftCollider.sidewaysFriction.stiffness;

        FRwheelFriction = new WheelFrictionCurve();
        FRwheelFriction.extremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
        FRWextremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
        FRwheelFriction.extremumValue = frontRightCollider.sidewaysFriction.extremumValue;
        FRwheelFriction.asymptoteSlip = frontRightCollider.sidewaysFriction.asymptoteSlip;
        FRwheelFriction.asymptoteValue = frontRightCollider.sidewaysFriction.asymptoteValue;
        FRwheelFriction.stiffness = frontRightCollider.sidewaysFriction.stiffness;

        RLwheelFriction = new WheelFrictionCurve();
        RLwheelFriction.extremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
        RLWextremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
        RLwheelFriction.extremumValue = rearLeftCollider.sidewaysFriction.extremumValue;
        RLwheelFriction.asymptoteSlip = rearLeftCollider.sidewaysFriction.asymptoteSlip;
        RLwheelFriction.asymptoteValue = rearLeftCollider.sidewaysFriction.asymptoteValue;
        RLwheelFriction.stiffness = rearLeftCollider.sidewaysFriction.stiffness;

        RRwheelFriction = new WheelFrictionCurve();
        RRwheelFriction.extremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
        RRWextremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
        RRwheelFriction.extremumValue = rearRightCollider.sidewaysFriction.extremumValue;
        RRwheelFriction.asymptoteSlip = rearRightCollider.sidewaysFriction.asymptoteSlip;
        RRwheelFriction.asymptoteValue = rearRightCollider.sidewaysFriction.asymptoteValue;
        RRwheelFriction.stiffness = rearRightCollider.sidewaysFriction.stiffness;

        // (3) ���� ���� �⺻ ��ġ�� ����
        if (carEngineSound != null)
        {
            initialCarEngineSoundPitch = carEngineSound.pitch;
        }

        // (4) ���� �ӵ� UI�� ���� ������Ʈ �޼��带 �ֱ������� ȣ��
        if (useUI)
        {
            InvokeRepeating("CarSpeedUI", 0f, 0.1f);
        }
        else if (!useUI)
        {
            if (carSpeedText != null)
            {
                carSpeedText.text = "0";
            }
        }

        if (useSounds)
        {
            InvokeRepeating("CarSounds", 0f, 0.1f);
        }
        else if (!useSounds)
        {
            if (carEngineSound != null)
            {
                carEngineSound.Stop();
            }
            if (tireScreechSound != null)
            {
                tireScreechSound.Stop();
            }
        }

        // (5) ����Ʈ ��� ���� ó��
        if (!useEffects)
        {
            if (RLWParticleSystem != null)
            {
                RLWParticleSystem.Stop();
            }
            if (RRWParticleSystem != null)
            {
                RRWParticleSystem.Stop();
            }
            if (RLWTireSkid != null)
            {
                RLWTireSkid.emitting = false;
            }
            if (RRWTireSkid != null)
            {
                RRWTireSkid.emitting = false;
            }
        }

        // (6) ��ġ ��Ʈ�� ��� ��, �� ��ư ���� Ȯ�� �� PrometeoTouchInput ��������
        if (useTouchControls)
        {
            if (throttleButton != null && reverseButton != null &&
                turnRightButton != null && turnLeftButton != null &&
                handbrakeButton != null)
            {
                throttlePTI = throttleButton.GetComponent<PrometeoTouchInput>();
                reversePTI = reverseButton.GetComponent<PrometeoTouchInput>();
                turnLeftPTI = turnLeftButton.GetComponent<PrometeoTouchInput>();
                turnRightPTI = turnRightButton.GetComponent<PrometeoTouchInput>();
                handbrakePTI = handbrakeButton.GetComponent<PrometeoTouchInput>();
                touchControlsSetup = true;
            }
            else
            {
                String ex = "��ġ ��Ʈ���� �ùٸ��� �������� �ʾҽ��ϴ�. PrometeoCarController ������Ʈ���� " +
                            "��ư���� ����� �巡�� �� ����ؾ� �մϴ�.";
                Debug.LogWarning(ex);
            }
        }
    }

    // --------------------------------------------------------------------------------------
    // Update() �޼���
    // �� �����Ӹ��� ȣ��Ǹ�, ����� �Է°� ���� ������ ó��
    // --------------------------------------------------------------------------------------
    void Update()
    {
        // (1) ���� �ӵ� ����
        carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;
        // (2) ���� ��ǥ�迡���� x, z�� �ӵ� ���
        localVelocityX = transform.InverseTransformDirection(carRigidbody.linearVelocity).x;
        localVelocityZ = transform.InverseTransformDirection(carRigidbody.linearVelocity).z;

        // (3) ���� ��Ʈ�� ���� - ��ġ ��Ʈ�� vs Ű����(PC)
        if (useTouchControls && touchControlsSetup)
        {
            // ����
            if (throttlePTI.buttonPressed)
            {
                CancelInvoke("DecelerateCar");
                deceleratingCar = false;
                GoForward();
            }
            // ����
            if (reversePTI.buttonPressed)
            {
                CancelInvoke("DecelerateCar");
                deceleratingCar = false;
                GoReverse();
            }
            // ��ȸ��
            if (turnLeftPTI.buttonPressed)
            {
                TurnLeft();
            }
            // ��ȸ��
            if (turnRightPTI.buttonPressed)
            {
                TurnRight();
            }
            // �ڵ�극��ũ
            if (handbrakePTI.buttonPressed)
            {
                CancelInvoke("DecelerateCar");
                deceleratingCar = false;
                Handbrake();
            }
            if (!handbrakePTI.buttonPressed)
            {
                RecoverTraction();
            }

            // ����X, ����X -> �ӵ� ����
            if ((!throttlePTI.buttonPressed && !reversePTI.buttonPressed))
            {
                ThrottleOff();
            }
            if ((!reversePTI.buttonPressed && !throttlePTI.buttonPressed) && !handbrakePTI.buttonPressed && !deceleratingCar)
            {
                InvokeRepeating("DecelerateCar", 0f, 0.1f);
                deceleratingCar = true;
            }

            // ��Ƽ� �� �ʱ�ȭ
            if (!turnLeftPTI.buttonPressed && !turnRightPTI.buttonPressed && steeringAxis != 0f)
            {
                ResetSteeringAngle();
            }
        }
        else
        {
            // PC Ű���� �Է�
            if (Input.GetKey(KeyCode.W))
            {
                CancelInvoke("DecelerateCar");
                deceleratingCar = false;
                GoForward();
            }
            if (Input.GetKey(KeyCode.S))
            {
                CancelInvoke("DecelerateCar");
                deceleratingCar = false;
                GoReverse();
            }
            if (Input.GetKey(KeyCode.A))
            {
                TurnLeft();
            }
            if (Input.GetKey(KeyCode.D))
            {
                TurnRight();
            }
            if (Input.GetKey(KeyCode.Space))
            {
                CancelInvoke("DecelerateCar");
                deceleratingCar = false;
                Handbrake();
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                RecoverTraction();
            }

            if ((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)))
            {
                ThrottleOff();
            }
            if ((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)) && !Input.GetKey(KeyCode.Space) && !deceleratingCar)
            {
                InvokeRepeating("DecelerateCar", 0f, 0.1f);
                deceleratingCar = true;
            }
            if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && steeringAxis != 0f)
            {
                ResetSteeringAngle();
            }
        }

        // (4) ���� �ݶ��̴��� ���� ����(3D �޽�)�� ��ġ/ȸ�� ����ȭ
        AnimateWheelMeshes();
    }

    // --------------------------------------------------------------------------------------
    // CarSpeedUI()
    // UI �ؽ�Ʈ�� ���� �ӵ��� ǥ���ϱ� ���� �޼���
    // --------------------------------------------------------------------------------------
    public void CarSpeedUI()
    {
        if (useUI)
        {
            try
            {
                float absoluteCarSpeed = Mathf.Abs(carSpeed);
                carSpeedText.text = Mathf.RoundToInt(absoluteCarSpeed).ToString();
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }
    }

    // --------------------------------------------------------------------------------------
    // CarSounds()
    // ���� ���带 �����ϴ� �޼��� (������ ��ġ, �帮��Ʈ ���� ��)
    // --------------------------------------------------------------------------------------
    public void CarSounds()
    {
        if (useSounds)
        {
            try
            {
                // ���� ���� ��ġ = �⺻ ��ġ + �ӵ� ��� ����
                if (carEngineSound != null)
                {
                    float engineSoundPitch = initialCarEngineSoundPitch + (Mathf.Abs(carRigidbody.linearVelocity.magnitude) / 25f);
                    carEngineSound.pitch = engineSoundPitch;
                }
                // �帮��Ʈ ����: �̲������ų�(�帮��Ʈ) �������� ���µ�(�ڵ�극��ũ) �ӵ��� ������� ���� ��
                if ((isDrifting) || (isTractionLocked && Mathf.Abs(carSpeed) > 12f))
                {
                    if (!tireScreechSound.isPlaying)
                    {
                        tireScreechSound.Play();
                    }
                }
                else if ((!isDrifting) && (!isTractionLocked || Mathf.Abs(carSpeed) < 12f))
                {
                    tireScreechSound.Stop();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }
        else if (!useSounds)
        {
            // ���� ��� �� ��
            if (carEngineSound != null && carEngineSound.isPlaying)
            {
                carEngineSound.Stop();
            }
            if (tireScreechSound != null && tireScreechSound.isPlaying)
            {
                tireScreechSound.Stop();
            }
        }
    }

    // --------------------------------------------------------------------------------------
    // TurnLeft()
    // �������� �ڵ��� ����
    // --------------------------------------------------------------------------------------
    public void TurnLeft()
    {
        steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
        if (steeringAxis < -1f)
        {
            steeringAxis = -1f;
        }
        var steeringAngle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    // --------------------------------------------------------------------------------------
    // TurnRight()
    // ���������� �ڵ��� ����
    // --------------------------------------------------------------------------------------
    public void TurnRight()
    {
        steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
        if (steeringAxis > 1f)
        {
            steeringAxis = 1f;
        }
        var steeringAngle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    // --------------------------------------------------------------------------------------
    // ResetSteeringAngle()
    // �ڵ��� �߾����� �ǵ���
    // --------------------------------------------------------------------------------------
    public void ResetSteeringAngle()
    {
        if (steeringAxis < 0f)
        {
            steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
        }
        else if (steeringAxis > 0f)
        {
            steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
        }
        if (Mathf.Abs(frontLeftCollider.steerAngle) < 1f)
        {
            steeringAxis = 0f;
        }
        var steeringAngle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    // --------------------------------------------------------------------------------------
    // AnimateWheelMeshes()
    // WheelCollider ������ �������� 3D �� �޽��� ��ġ �� ȸ���� ����ȭ
    // --------------------------------------------------------------------------------------
    void AnimateWheelMeshes()
    {
        try
        {
            Quaternion FLWRotation;
            Vector3 FLWPosition;
            frontLeftCollider.GetWorldPose(out FLWPosition, out FLWRotation);
            frontLeftMesh.transform.position = FLWPosition;
            frontLeftMesh.transform.rotation = FLWRotation;

            Quaternion FRWRotation;
            Vector3 FRWPosition;
            frontRightCollider.GetWorldPose(out FRWPosition, out FRWRotation);
            frontRightMesh.transform.position = FRWPosition;
            frontRightMesh.transform.rotation = FRWRotation;

            Quaternion RLWRotation;
            Vector3 RLWPosition;
            rearLeftCollider.GetWorldPose(out RLWPosition, out RLWRotation);
            rearLeftMesh.transform.position = RLWPosition;
            rearLeftMesh.transform.rotation = RLWRotation;

            Quaternion RRWRotation;
            Vector3 RRWPosition;
            rearRightCollider.GetWorldPose(out RRWPosition, out RRWRotation);
            rearRightMesh.transform.position = RRWPosition;
            rearRightMesh.transform.rotation = RRWRotation;
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }

    // --------------------------------------------------------------------------------------
    // GoForward()
    // ������ ���� ���� ��ũ(���� ��) ����
    // --------------------------------------------------------------------------------------
    public void GoForward()
    {
        // x�� �������� ���� ũ�� �ɸ��� �帮��Ʈ ���·� �ν�
        if (Mathf.Abs(localVelocityX) > 2.5f)
        {
            isDrifting = true;
            DriftCarPS();
        }
        else
        {
            isDrifting = false;
            DriftCarPS();
        }

        // ���ӵ� ���� �ε巴�� 1���� ����
        throttleAxis = throttleAxis + (Time.deltaTime * 3f);
        if (throttleAxis > 1f)
        {
            throttleAxis = 1f;
        }

        // ���� ������ ���� ���̸� -> �극��ũ �ɾ�� ��
        if (localVelocityZ < -1f)
        {
            Brakes();
        }
        else
        {
            // �ִ� �ӵ� ������ ���� ��ũ�� �ش�
            if (Mathf.RoundToInt(carSpeed) < maxSpeed)
            {
                frontLeftCollider.brakeTorque = 0;
                frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                frontRightCollider.brakeTorque = 0;
                frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearLeftCollider.brakeTorque = 0;
                rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearRightCollider.brakeTorque = 0;
                rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
            }
            else
            {
                // �ִ� �ӵ��� �����ϸ� ��ũ�� 0����
                frontLeftCollider.motorTorque = 0;
                frontRightCollider.motorTorque = 0;
                rearLeftCollider.motorTorque = 0;
                rearRightCollider.motorTorque = 0;
            }
        }
    }

    // --------------------------------------------------------------------------------------
    // GoReverse()
    // ������ ���� ���� ��ũ(���� ��) ����
    // --------------------------------------------------------------------------------------
    public void GoReverse()
    {
        // x�� �������� ���� ũ�� �ɸ��� �帮��Ʈ ���·� �ν�
        if (Mathf.Abs(localVelocityX) > 2.5f)
        {
            isDrifting = true;
            DriftCarPS();
        }
        else
        {
            isDrifting = false;
            DriftCarPS();
        }

        // ���ӵ� ���� �ε巴�� -1���� ����
        throttleAxis = throttleAxis - (Time.deltaTime * 3f);
        if (throttleAxis < -1f)
        {
            throttleAxis = -1f;
        }

        // ���� ���̸� -> �극��ũ
        if (localVelocityZ > 1f)
        {
            Brakes();
        }
        else
        {
            // �ִ� ���� �ӵ� ������ ���� ��ũ ����
            if (Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed)
            {
                frontLeftCollider.brakeTorque = 0;
                frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                frontRightCollider.brakeTorque = 0;
                frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearLeftCollider.brakeTorque = 0;
                rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearRightCollider.brakeTorque = 0;
                rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
            }
            else
            {
                // �ִ� ���� �ӵ��� �����ϸ� ��ũ 0
                frontLeftCollider.motorTorque = 0;
                frontRightCollider.motorTorque = 0;
                rearLeftCollider.motorTorque = 0;
                rearRightCollider.motorTorque = 0;
            }
        }
    }

    // --------------------------------------------------------------------------------------
    // ThrottleOff()
    // ������ ����(��ũ 0)
    // --------------------------------------------------------------------------------------
    public void ThrottleOff()
    {
        frontLeftCollider.motorTorque = 0;
        frontRightCollider.motorTorque = 0;
        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;
    }

    // --------------------------------------------------------------------------------------
    // DecelerateCar()
    // ���� ���� ���� (InvokeRepeating���� �ֱ������� �ҷ��� ������ ������ ����)
    // --------------------------------------------------------------------------------------
    public void DecelerateCar()
    {
        // x�� �������� ���� ũ�� �ɸ��� �帮��Ʈ ���·� �ν�
        if (Mathf.Abs(localVelocityX) > 2.5f)
        {
            isDrifting = true;
            DriftCarPS();
        }
        else
        {
            isDrifting = false;
            DriftCarPS();
        }

        // throttleAxis�� �ε巴�� 0���� ����
        if (throttleAxis != 0f)
        {
            if (throttleAxis > 0f)
            {
                throttleAxis = throttleAxis - (Time.deltaTime * 10f);
            }
            else if (throttleAxis < 0f)
            {
                throttleAxis = throttleAxis + (Time.deltaTime * 10f);
            }
            if (Mathf.Abs(throttleAxis) < 0.15f)
            {
                throttleAxis = 0f;
            }
        }

        // Rigidbody �ӵ��� decelerationMultiplier�� ����ؼ� ����
        carRigidbody.linearVelocity = carRigidbody.linearVelocity * (1f / (1f + (0.025f * decelerationMultiplier)));

        // ���� ��ũ ����
        frontLeftCollider.motorTorque = 0;
        frontRightCollider.motorTorque = 0;
        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;

        // ���� ���� �ӵ�(0.25f ����)�� ������ ����
        if (carRigidbody.linearVelocity.magnitude < 0.25f)
        {
            carRigidbody.linearVelocity = Vector3.zero;
            CancelInvoke("DecelerateCar");
        }
    }

    // --------------------------------------------------------------------------------------
    // Brakes()
    // ������ �극��ũ ��ũ ����
    // --------------------------------------------------------------------------------------
    public void Brakes()
    {
        frontLeftCollider.brakeTorque = brakeForce;
        frontRightCollider.brakeTorque = brakeForce;
        rearLeftCollider.brakeTorque = brakeForce;
        rearRightCollider.brakeTorque = brakeForce;
    }

    // --------------------------------------------------------------------------------------
    // Handbrake()
    // ���̵�극��ũ(�ڵ�극��ũ)�� ����Ͽ� �������� ��ȭ -> �帮��Ʈ ����
    // --------------------------------------------------------------------------------------
    public void Handbrake()
    {
        CancelInvoke("RecoverTraction");

        // driftingAxis�� 0���� 1���� ������ �������� �ִ�� �̲������� ���¸� ����
        driftingAxis = driftingAxis + (Time.deltaTime);
        float secureStartingPoint = driftingAxis * FLWextremumSlip * handbrakeDriftMultiplier;

        if (secureStartingPoint < FLWextremumSlip)
        {
            driftingAxis = FLWextremumSlip / (FLWextremumSlip * handbrakeDriftMultiplier);
        }
        if (driftingAxis > 1f)
        {
            driftingAxis = 1f;
        }

        // x�� ���� ũ�� �帮��Ʈ ��
        if (Mathf.Abs(localVelocityX) > 2.5f)
        {
            isDrifting = true;
        }
        else
        {
            isDrifting = false;
        }

        // �̲����� ����
        if (driftingAxis < 1f)
        {
            FLwheelFriction.extremumSlip = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            frontLeftCollider.sidewaysFriction = FLwheelFriction;

            FRwheelFriction.extremumSlip = FRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            frontRightCollider.sidewaysFriction = FRwheelFriction;

            RLwheelFriction.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            rearLeftCollider.sidewaysFriction = RLwheelFriction;

            RRwheelFriction.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            rearRightCollider.sidewaysFriction = RRwheelFriction;
        }

        // �ڵ�극��ũ ��� �� -> Ʈ���� ���
        isTractionLocked = true;
        DriftCarPS();
    }

    // --------------------------------------------------------------------------------------
    // DriftCarPS()
    // ������ �帮��Ʈ�� �� ��ƼŬ/Ʈ���� ȿ���� ����
    // --------------------------------------------------------------------------------------
    public void DriftCarPS()
    {
        if (useEffects)
        {
            try
            {
                if (isDrifting)
                {
                    RLWParticleSystem.Play();
                    RRWParticleSystem.Play();
                }
                else if (!isDrifting)
                {
                    RLWParticleSystem.Stop();
                    RRWParticleSystem.Stop();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }

            try
            {
                if ((isTractionLocked || Mathf.Abs(localVelocityX) > 5f) && Mathf.Abs(carSpeed) > 12f)
                {
                    RLWTireSkid.emitting = true;
                    RRWTireSkid.emitting = true;
                }
                else
                {
                    RLWTireSkid.emitting = false;
                    RRWTireSkid.emitting = false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }
        else if (!useEffects)
        {
            if (RLWParticleSystem != null)
            {
                RLWParticleSystem.Stop();
            }
            if (RRWParticleSystem != null)
            {
                RRWParticleSystem.Stop();
            }
            if (RLWTireSkid != null)
            {
                RLWTireSkid.emitting = false;
            }
            if (RRWTireSkid != null)
            {
                RRWTireSkid.emitting = false;
            }
        }
    }

    // --------------------------------------------------------------------------------------
    // RecoverTraction()
    // �ڵ�극��ũ ���� ��, �ٽ� ���� ���������� �ǵ����� �Լ�
    // --------------------------------------------------------------------------------------
    public void RecoverTraction()
    {
        isTractionLocked = false;
        driftingAxis = driftingAxis - (Time.deltaTime / 1.5f);
        if (driftingAxis < 0f)
        {
            driftingAxis = 0f;
        }

        // ������ ���������� �ǵ��ƿ� ������ ������ ����
        if (FLwheelFriction.extremumSlip > FLWextremumSlip)
        {
            FLwheelFriction.extremumSlip = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            frontLeftCollider.sidewaysFriction = FLwheelFriction;

            FRwheelFriction.extremumSlip = FRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            frontRightCollider.sidewaysFriction = FRwheelFriction;

            RLwheelFriction.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            rearLeftCollider.sidewaysFriction = RLwheelFriction;

            RRwheelFriction.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            rearRightCollider.sidewaysFriction = RRwheelFriction;

            Invoke("RecoverTraction", Time.deltaTime);
        }
        else if (FLwheelFriction.extremumSlip < FLWextremumSlip)
        {
            FLwheelFriction.extremumSlip = FLWextremumSlip;
            frontLeftCollider.sidewaysFriction = FLwheelFriction;

            FRwheelFriction.extremumSlip = FRWextremumSlip;
            frontRightCollider.sidewaysFriction = FRwheelFriction;

            RLwheelFriction.extremumSlip = RLWextremumSlip;
            rearLeftCollider.sidewaysFriction = RLwheelFriction;

            RRwheelFriction.extremumSlip = RRWextremumSlip;
            rearRightCollider.sidewaysFriction = RRwheelFriction;

            driftingAxis = 0f;
        }
    }
}
