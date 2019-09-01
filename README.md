## 유니티 간단한 캐릭터 이동 스크립트 (19/08/20)

유니티에 기본적으로 입력되어 있는 "Horizontal", "Vertical" Input을 사용해 간단하게

캐릭터를 이동시키는 스크립트를 구현해 봄.

## Quaternion

Unity 내부적으로 회전 연산을 살 때 사용하는 객체

## Rigidbody

오브젝트의 물리를 표현할 수 있는 컴포넌트

Rigidbody 내 Constraints( 제한 사항들)에서 Rigidbody( 실질적 물체 )의 이동 및 회전을 제한할 수 있음

## Collider

오브젝트의 물리적 형태를 표현하는 컴포넌트

가져온(Import) 에셋 오브젝트의 경우 Collider가 존재하지 않을 수 있음

기본적으로 존재하는 Collider를 에셋 오브젝트에 적용할 경우, PlayerController_MainObject.cs는 좌우 회전이 작동되지 않음

이는 가져온 에셋 오브젝트에 맞는 Collider가 아니기 때문임

그렇기 때문에 Collider를 회전시키지 않고 물체 자체를 회전시키도록 함

-> PlayerController_Asset.cs

## 유니티 심화된 캐릭터 이동 스크립트 (19/08/21)

앞의 내용에 추가적으로 캐릭터가 달리거나 점프할 수 있고 앉을 수도 있도록 스크립트에 추가적으로

함수를 구현해 봄.

## Physics.Raycast 함수

주어진 위치에서 특정 방향으로 특정 길이만큼 레이저를 쏴 충돌하는 Collider가 있는지 확인하는 함수

해당 함수를 통해 캐릭터 물체가 지면에 닿아 있는지를 판별함

기본적인 오브젝트인 캡슐의 경우, Collider와 오브젝트의 크기가 같아 물체의 중앙값이 같지만

Collider가 없는 에셋 오브젝트의 경우 Capsule Collider를 적용하면 실제 중앙값과의 위치가 다르기 때문에

조정이 필요함.

## Coroutine 함수와 고간 함수 

코루틴( Coroutine )
	
	Caller가 함수를 Call하고, 함수가 Caller에게 값 을 return하면서 종료하는 것에 더해 return하는

	대신 suspend( 혹은 yield )하면 Caller가 나중에 resume하여 중단된 지점부터 실행을 이어갈 수

	있다.

	* Thread와 유사하지만 다름.

	Thread : 비동기 방식 -> Thread 들이 동시에 작동 ( 정확하게 동시는 아니지만 )
	
	Coroutine : 동기화 방식 -> Coroutine은 동시에 발생하지 않음

기존 앉기 방식은 카메라의 위치를 바로 앉는 상태로 변경하기 때문에 동작이 부드럽지 못함.

그렇기 때문에 앉는 상태에서 서 있는 상태, 서 잇는 상태에서 앉은 상태로 변경하는데 부자연스러운 모습이

보이게 됨.

그렇기 때문에 코루틴 함수와 Mathf 객체의 Lerp 함수를 사용해 부드러운 카메라 이동을 구현함

## 캐릭터 몸체에 팔 부착 및 팔 동작 구현

Edit > Project Setting > Input
	
	Unity 내에서 사용되는 입력 키 값을 정의할 수 있음

Camera ( 카메라 )

	Camera 컴포넌트의 Clear Flags -> Depth only
		
		Depth -> 0

			> 카메라들의 우선순위를 설정할 수 있음

			> 0일 경우, 가장 우선순위가 높음

		Near -> 0 ( 0.01로 변경됨 )
			
			> 카메라의 시야 범위를 줄일 수 있음

			> 팔이 움직일 때, 화면에서 잘려 보이는 것을 방지하기 위해 사용

	Audio Listener 컴포넌트

			> 현재 카메라에서 생성되는 소리를 설정하는 컴포넌트

			> 팔 설정을 위해 카메라를 두 개 만들었기 때문에 둘 중 하나를 제거

Raycast
	
	> Raycast 레이저는 오브젝트의 회전 이전, 기존의 방향에서 발사됨

		( 초기 팔의 방향이 맞지 않아 회전시켰음)

	> HandHolder의 오브젝트 Y축 각도를 90도 틀었으므로 기본 각도에서 레이저를 발사할 수 있도록

	상위 오브젝트를 생성해 설정함

## CrossHair

	> HUD의 크로스 헤어가 캐릭터의 움직임에 따라 변경될 수 있도록 움직임 관련 함수를 수정

## WeaponManager와 관련

	> WeaponManager 스크립트와 관련해 HandController, PlayerController 스크립트를 수정함

	> 자세한 사항은 Unity_Weapons 프로젝트에 위치
