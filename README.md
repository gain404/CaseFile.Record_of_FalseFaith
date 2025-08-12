# 사건파일 : 위신록(僞信錄)

<img width="1024" height="683" alt="Image" src="https://github.com/user-attachments/assets/7abe0cb2-d0fc-4e2b-a7af-329e4bb2fe91" />

안녕하세요,
2D 횡스크롤 장르의 한국풍 퇴마게임, 사건파일 : 위신록을 제작한 팀 가온🍀입니다.

## 목차

1. [📜 프로젝트 개요](#프로젝트-개요)
2. [🕹️ 게임 로직](#게임-로직)
3. [🗺️ 와이어프레임 및 초기 구상](#와이어프레임-및-초기-구상)
5. [🛠️ 사용 기술](#사용-기술)
6. [👥 팀원 소개](#팀원-소개)

## 📜 프로젝트 개요

더 이상 허상의 존재가 아닌 '악귀'.  
2067년의 서울에서 '초자연범죄대응부'의 형사 '강서현'이 되어 악귀들에 의한 범죄 및 자연현상들을 수사하고 봉인하여 없애세요.

![Image](https://github.com/user-attachments/assets/ebba537f-4588-40b9-97f2-63ed7059acb1)
![Image](https://github.com/user-attachments/assets/abbf8333-a843-4c7e-9a67-f913d0ec8213)

|게임명| 사건파일 : 위신록 |
|-----|--------------------|
|장르|2D 횡스크롤 조사 액션 게임|
|개발 플랫폼| Unity 6000.1.8f1|
|개발 기간| 2025.6.17~2025.8.12 |
|팀원| 김가인, 조은서, 박준아, 강민성, 송도현 |

## 🕹️ 게임 로직

<img width="450" height="250" alt="Image" src="https://github.com/user-attachments/assets/9dfdaf8b-38ac-4c06-8719-d0c66c2e3465" />  

방향키를 통해 이동하고, F를 통해 조사합니다.  

<img width="450" height="250" alt="Image" src="https://github.com/user-attachments/assets/e9c98f05-dad2-4b3d-8c40-59a2caf7471e" />  

Tab을 눌러 인벤토리를 열 수 있습니다.  

<img width="450" height="250" alt="Image" src="https://github.com/user-attachments/assets/34d54975-e9c8-41be-b8b4-50504dc6c907" />  

조사를 통해 습득한 아이템 중 일부는 npc'도하'에게 가져가 조사를 맡길 수 있습니다.  
조사를 맡긴 아이템의 조사가 끝나면, 오른쪽 상단의 조사파일에서 조사된 내용을 확인할 수 있습니다.  

## 🗺️ 와이어프레임 및 초기 구상
와이어프레임(초기 컨셉)
<img width="1589" height="730" alt="image" src="https://github.com/user-attachments/assets/3fc0adc4-8fe5-4a3f-b859-bb82f642d0c3" /><br>

와이어프레임(몬스터)

<img width="1654" height="661" alt="image" src="https://github.com/user-attachments/assets/275bb72a-1a91-459e-8fde-ef50e6194af9" /><br>

와이어프레임(UI 초안)

<img width="1329" height="690" alt="image" src="https://github.com/user-attachments/assets/f5d1bbcd-7bba-4dcc-bb7d-f65156290581" /><br>
<img width="1340" height="729" alt="image" src="https://github.com/user-attachments/assets/a10e1fd4-6bd5-49d0-a82f-8d91be5953ab" /><br>

와이어프레임(조사관련)
<img width="1365" height="739" alt="image" src="https://github.com/user-attachments/assets/18cdb89e-9f27-426f-a566-8cd8361b5b26" /><br>

와이어프레임(맵)
<img width="1667" height="650" alt="image" src="https://github.com/user-attachments/assets/7199cee3-2642-4e1c-9a7b-2c918b796ba8" /><br>


## 🛠️ 사용 기술

- **Unity Engine (Unity 6)**
  - 2D & 3D 혼합 개발 환경 활용
  - UGUI 기반 UI 시스템 구현
- **플레이어 FSM (Finite State Machine)**
  - 상태 전환 기반 캐릭터 행동 제어
  - 이동, 점프, 공격, 대기 등의 상태를 명확하게 구분 및 관리
- **몬스터 AI : Unity Behavior System**
  - Unity 6 Behavior Graph 기반 AI 로직 구성
  - 탐지, 추격, 공격, 복귀 등 행동 패턴 설계
- **싱글톤 패턴**
  - UI, 데이터, 매니저 클래스의 전역 접근 및 유지
  - 씬 전환 시 데이터 유지(DontDestroyOnLoad) 처리
- **DOTween**
  - UI 전환, 페이드 인/아웃, 오브젝트 애니메이션 효과 구현
- **Timeline**
  - 컷신 및 NPC 연출 제작
- **CSV 데이터 관리**
  - 게임 데이터(대사, 설정 등)를 CSV로 관리
  - 런타임에 파싱하여 딕셔너리/리스트 구조로 로드
- **오브젝트 풀링(Object Pooling)**
  - 몬스터, 투사체 등 반복 생성/삭제 오브젝트 성능 최적화
- **Cinemachine**
  - 카메라 추적, 영역 전환, 연출 카메라 구현

## 👥 팀원 소개
| 이름 | 역할 | 담당 기능 | 주요 기여 |
|------|------|----------|-----------|
| 김가인 | 팀장 / 기획 / 프로그래머 | 게임 기획, 플레이어 FSM | 스토리 구성, FSM 설계 및 구현 |
| 조은서 | 기획 / 아트 | 배경/캐릭터 디자인/게임 기획 | 스토리 구성, 픽셀 아트 제작, 애니메이션 작업 |
| 강민성 | 프로그래머 / QA | 대화/조사/지도 시스템/버그 픽스 | UnityUI를 이용하여 대화 및 조사 개발 |
| 박준아 | 프로그래머 / QA | 몬스터 AI/UIManager/조사UI/리펙토링 | Unity Behavior를 이용하여 몬스터 AI개발, UI관리 |
| 송도현 | 프로그래머 / QA | 인벤토리/세이브로드/SoundManager/타이틀 | 타이틀 Scene제작, Json을 이용하여 세이브 개발 |
