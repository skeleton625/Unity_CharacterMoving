using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Asset : MonoBehaviour
{
    /* 
     * SerializeField - private은 유지되면서 인스펙터 창에 표시되는 변수 값 변경이 가능
     * * 단 모든 변수가 인스펙터 창에 표시되는 것은 아님
     */
        // 스피드 조정 변수들
    [SerializeField]
    /* 캐릭터의 걷는 속도 */
    private float walkSpeed;
    [SerializeField]
    /* 캐릭터의 달리기 속도 */
    private float runSpeed;
    /* 실제 플레이어가 이동하는 속도 */
    private float applySpeed;
    /* 달리고 있는지 유무 */
    private bool isRun;     

        // 캐릭터 점프 변수들
    [SerializeField]
    /* 캐릭터의 점프 정도 */
    private float jumpForce;
    /* 땅에 붙어있는지 유무 */
    private bool isGround;

        // 캐릭터 앉기 변수들
    [SerializeField]
    /* 캐릭터의 앉은 후 Y 값 */
    private float CrouchPosY;
    [SerializeField]
    /* 캐릭터가 앉는 속도 */
    private float crouchSpeed;
    /* 캐릭터의 앉기 전 Y 값 */
    private float originPosY;
    /* 실제 케릭터에게 적용된 Y 값 */
    private float applyCrouchPosY;
    private bool isCrouch;  /* 앉아 있는지 유무 */

        // 마우스 입력에 의한 변수들
    [SerializeField]
    /* 캐릭터의 마우스 민감도 */
    private float lookSensitivity;
    [SerializeField]
    /* 캐릭터 화면 회전 제한 (너무 많은 각도로 회전하지 않게 하기 위함) */
    private float cameraRotationLimit;
    /* 캐릭터 화면 가로 축의 현재 회전 각도 */
    private float currentCameraRotationX = 0.0f;
    /* 캐릭터의 현재 Y축 회전 각도 */
    private float currentCharacterRotationY = 0.0f;

        // 컴포넌트, 게임 오브젝트 변수들
    [SerializeField]
    /* 
     * 캐릭터의 화면( 카메라 ) 객체 
     * 여러 개의 카메라 중, 특정 카메라만 선택하기 위해 인스펙터 창에서 직접 넣어줄 것임
     */
    private Camera theCamera;
    /* 캐릭터의 물리적 몸체 - 충돌 영역 */
    private Rigidbody myRigid;
    /* 캐릭터의 충돌 영역 */
    private CapsuleCollider CharacterCollider;

    // Start is called before the first frame update
    void Start()
    {
        /* Script가 넣어진 오브젝트의 컴포넌트들 중, Temprate에 해당하는 컴포넌트들을 적용 */
        CharacterCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        /* 처음 시작했을 때의 초기 상태는 걷는 상태 */
        applySpeed = walkSpeed;
        /* 
         * 실제 화면에서 앉아 있는지 아닌지로 규정하기 위함
         * World Position, Local Position은 다르다 -> 컴퓨터 그래픽스 Position 축
         */
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;
    }

    // 매 프레임( 초당 60 프레임 )마다 실행되는 함수
    void Update()
    {     
        /* 캐릭터가 지면에 있는지 공중에 있는지 파악 */
        SetIsGround();
        /* 키 입력에 따른 캐릭터 점프 시도 */
        TryJump();
        /* 앉은 상태와 서 있는 상태 변경 시도 */
        TryCrouch();
        /* 키 입력에 따른 캐릭터 달리기 시도 */
        TryRun();
        /* 캐릭터 전 후 좌 우 이동 */
        Move();
        /* 마우스 입력에 따른 플레이어 화면(카메라) 상 하 이동 */
        CameraRotation();
        /* 마우스 입력에 따른 캐릭터, 카메라 좌 우 회전 */
        CharacterRotation();
    }

    /* 전후 좌우 캐릭터 이동 */
    private void Move()
    {
        /* 
         * Unity에 명시되어 있는 Horizontal 키
         * 키에 따른 수평 방향 값을 반환
         */
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        /*
         * Unity에 명시되어 있는 Vertical 키
         * 키에 따른 수직 방향 값을 반환
         */
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        /* transform.right = (1, 0, 0) */   
        Vector3 _moveHorizontal = transform.right * _moveDirX;
        /* transform.forward = (0, 0, 1) */
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        /*
         * 수평 입력 값과 수직 입력 값을 더함
         * 연산의 단순화를 위해 더한 Vector3를 표준화 함
         * 표준화된 값에 캐릭터 속도를 곱함
         * => 캐릭터의 이동 속도
         * 08/21 -> walkSpeed를 applySpeed로 변경 ( 적용할 캐릭터 속도 변수를 하나로 통일하기 위함 )
         */
        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

        /* 
         * Time.deltaTime -> 1 프레임의 대략적인 시간
         * 1초( 60 프레임 )동안 _velocity 만큼의 위치를 이동하게 됨
         * Asset에 맞는 Collider가 없을 시, Capsule Collider를 사용하면 이동 가능
         */
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    /* 상하 카메라 이동 */
    private void CameraRotation()
    {
        /* 마우스의 상 하 방향 변수 값 */
        float _XRotation = Input.GetAxisRaw("Mouse Y");
        /* 마우스의 민감 정도를 적용해 카메라 회전 정도를 정의 */
        float _cameraRotationX = _XRotation * lookSensitivity;
        /*
         * 카메라의 상 하 방향 입력에 대해 반전 시켜줌
         * 실제 마우스 이동대로 카메라 시선이 이동할 것임
         */
        currentCameraRotationX -= _cameraRotationX;
        /* 
         * currentCameraRotationX 변수 값을 특정 영역에 고정 
         * ( 고정할 값 , 최소 고정 값, 최대 고정 값 ) 
         */
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        /* 
         * 계산이 완료된 회전 값을 카메라 객체에 적용
         * transform.localEulerAngles => 해당 오브젝트의 회전 값
         */
        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0.0f, 0.0f);
    }

    private void CharacterRotation()
    {
        /* 마우스의 좌 우 방향 변수 값 */
        float _yRotation = Input.GetAxisRaw("Mouse X");
        /* 마우스 민감 정도를 적용 */
        float _characterRotationY = _yRotation * lookSensitivity;

        currentCharacterRotationY += _characterRotationY;
        /* 
         * Asset에 맞는 Collider가 없을 시,
         * 기존 Rigidbody를 이용산 회전이 불가능함
         * Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
         * yRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
         */
        /* Rigidbody가 아닌 물체 자체를 이동 */
        gameObject.transform.localEulerAngles = new Vector3(0.0f,currentCharacterRotationY, 0.0f);
    }

    // 달리는 것을 시도하는 변수
    private void TryRun()
    {
        /* 달리는 키를 눌렀을 경우, 달리기 시작 */
        if(Input.GetKey(KeyCode.LeftShift) && !isCrouch)
        {
            Running();
        }
        /* 달리는 키를 땟을 경우, 달리기 종료 */
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            StopRunning();
        }
    }

    // 캐릭터의 점프를 시도하는 함수
    private void TryJump()
    {
        // 만일 캐릭터가 지면에 닿아 있고 특정 키를 눌렀을 경우, 캐릭터가 점프하도록 함
        if(Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
        }
    }

    // 달리기를 적용하는 함수
    private void Running()
    {
        /* 달리기 상태를 True로 변경 */
        isRun = true;
        /* 적용된 캐릭터 속도를 달리기로 변경 */
        applySpeed = runSpeed;
    }
    
    // 달리기를 중지하는 함수
    private void StopRunning()
    {
        isRun = false;
        applySpeed = walkSpeed;
    }

    // 캐릭터가 점프하는 함수
    private void Jump()
    {
        /* 
         * Rigidbody.velocity : 해당 물체의 이동하는 속도
         * trasnform.up : (0, 1, 0)
         */
        myRigid.velocity = transform.up * jumpForce;
        isGround = false;
    }

    private void SetIsGround()
    {
        /*
         * Physics.Rascast
         *      주어진 특정 위치에서 주어진 다른 위치로 레이저를 쏨
         *      해당 물체가 떠 있는지 떠 있지 않은지 알기 위함
         * 에셋 캐릭터의 경우, 캡슐 콜리더의 위치를 이동시켜 사용하기 때문에 기존 방식이 잘 적용되지 않음
         * CharacterCollider.bounds.extents.y 값은 0.5가 되지만 실제 Collider의 센터 값은 0 이기 때문
         */
        isGround = Physics.Raycast(transform.position, Vector3.down, 0.1f);
    }

    // 앉기를 시도하는 함수
    private void TryCrouch()
    {
        /* 키 입력을 통해 앉은 상태와 서 있는 상태를 변경 */
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    // 앉은 상태, 서 있는 상태 변경 함수
    private void Crouch()
    {
        /* 앉은 상태의 변형 */
        isCrouch = !isCrouch;
        /* 앉아 있을 경우, 앉은 속도와 카메라 위치 변경 */
        if(isCrouch)
        {
            applySpeed = crouchSpeed;
            applyCrouchPosY = CrouchPosY;
        }
        /* 서 잇을 경우, 서 있는 속도(걷는 속도)와 카메라 위치 변경 */
        else
        {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }

        /* 카메라의 지역 위치에서 Y 축 위치만 변경함 */
        theCamera.transform.localPosition = new Vector3(theCamera.transform.localPosition.x, applyCrouchPosY, theCamera.transform.localPosition.z);
    }
}

