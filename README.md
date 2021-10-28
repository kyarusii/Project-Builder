# Project-Builder

미리 지정해둔 여러가지 옵션으로 빌드를 수행해주는 간단한 유틸리티입니다.  

## Install

```json
{
    "dependencies": {
        "com.calci.projectbuilder": "1.2.4"
    }
}
```

```json
{
    "scopedRegistries": [
        {
            "name": "npm",
            "url": "https://registry.npmjs.org",
            "scopes": [
                "com.calci"
            ]
        }
    ]
}
```

## Fundamentals
기능 구현 벤치마크 및 프로파일링을 위해 가능한 모든 조합으로 빌드를 해야 할 일이 생겨 제작된 도구입니다. 
단순히 모든 옵션으로 빌드를 하는 것 외에도 Deploy 환경에 따라 사용자 지정 옵션으로 드가 필요할 때 유용합니다.
클라이언트-서버, 크로스 플랫폼 등 여러 선택지 대응이 목표입니다.

## Usage
- `BuildProfile` : 하나의 바이너리 빌드를 생성하는 정의 프로필입니다.  
- `BuildCollection` : 여러개의 `BuildProfile`을 포함하여 다른 옵션의 빌드를 순차적으로 수행하기 위한 컬렉션입니다.  
- `PB_MENUITEM` : 프로젝트 설정의 `PlayerSetting` 에서 `Scipting Symbol`에 추가해주면 기본 메뉴아이템이 노출됩니다.  

### Wizard
`Window/Project Builder Wizard`

<p align="center">
<img src="https://user-images.githubusercontent.com/79823287/139198519-8a37da32-00db-4503-aec6-3bb04531546c.png" width="600">
</p>

### Build Profile
<p align="center">
<img src="https://user-images.githubusercontent.com/79823287/139209341-41c8e9cb-4d3e-4635-9783-26dfb3614edc.png" width="600">
</p>

- `Expose To Wizard` : 비활성화시 프로젝트 단위 검색에서 제외됩니다.  
- `Build Path` : 미리 지정된 심볼을 이용해 빌드 경로를 지정할 수 있습니다.   
    - `{ProjectRoot}` : 프로젝트 루트 (Assets 폴더가 위치한 디렉터리로 대체)  
    - `{Platform}` : 빌드된 플랫폼 (ex. Windows = Standalone)  
    - `{ProfileName}` : 프로필 이름 (ex. client_il2cpp_release.asset 프로필 = client_il2cpp_release)  
    - `{ProductName}` : 애플리케이션 이름 (`Application.productName`으로 대체)  
- `Headless` : 윈도우 플랫폼에서 그래픽스 API를 사용하지 않는 Console 모드로 빌드 (서버 빌드)  

### API

```csharp
public static void BuildPlayer(eBackendType backend, eBuildType buildType, eShippingType shippingType)
```

### MenuItem
 ![image](https://user-images.githubusercontent.com/79823287/122322590-a95a2780-cf60-11eb-8116-bb1efc103fed.png)
