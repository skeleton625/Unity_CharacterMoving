using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_MainObject : MonoBehaviour
{
    /* 
     * SerializeField - private은 유지되면서 인스펙터 창에 표시되는 변수 값 변경이 가능
     * * 단 모든 변수가 인스펙터 창에 표시되는 것은 아님
     */
    [SerializeField]
    /* 캐릭터의 이동 속도 */
    private float walkSpeed;
    [SerializeField]
    /* 캐릭터의 마우스 민감도 */
    private float lookSensitivity;

    [SerializeField]
    /* 캐릭터 화면 회전 제한 (너무 많은 각도로 회전하지 않게 하기 위함) */
    private float cameraRotationLimit;
    /* 캐릭터 화면 가로 축의 현재 회전 각도 */
    private float currentCameraRotationX = 0.0f;

    [SerializeField]
    /* 
     * 캐릭터의 화면( 카메라 ) 객체 
     * 여러 개의 카메라 중, 특정 카메라만 선택하기 위해 인스펙터 창에서 직접 넣어줄 것임
     */
    private Camera theCamera;

    /* 캐릭터의 물리적 몸체 - 충돌 영역 */
    private Rigidbody myRigid;

    // Start is called before the first frame update
    void Start()
    {
        /* Script가 넣어진 오브젝트의 컴포넌트들 중, Rigidbody를 가져옴 */
        myRigid = GetComponent<Rigidbody>();
    }

    // 매 프레임( 초당 60 프레임 )마다 실행되는 함수
    void Update()
    {
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
         */
        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * walkSpeed;

        /* 
         * Time.deltaTime -> 1 프레임의 대략적인 시간
         * 1초( 60 프레임 )동안 _velocity 만큼의 위치를 이동하게 됨
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
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        float _charRotationY = _yRotation * lookSensitivity;
        /* 
         * 해당 오브젝트의 Rigidbody 회전에 좌 우 방향을 적용
         * Unity는 내부적으로 회전에 Quaternion을 사용함
         * Vector3 값을 Quaternion으로 변형
         */
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
    }
}
