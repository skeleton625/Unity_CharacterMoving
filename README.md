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
