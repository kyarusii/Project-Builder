# Project-Builder

미리 지정해둔 여러가지 옵션으로 빌드를 수행해주는 간단한 유틸리티입니다.  

## Install

```json
{
    "dependencies": {
        "com.calci.project-builder": "1.2.0"
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
단순히 모든 옵션으로 빌드를 하는 것 외에도 Deploy 환경에 따라 사용자 지정 옵션으로 필드가 필요할 때 유용합니다.
클라이언트-서버, 크로스 플랫폼 등 여러 선택지 대응이 목표입니다.

## API

```csharp
public static void BuildPlayer(eBackendType backend, eBuildType buildType, eShippingType shippingType)
```

```csharp
 internal enum eBackendType
 {
     MONO,
     IL2CPP,
     IL2CPP_SOLUTION,
 }

 internal enum eShippingType
 {
     RELEASE,
     DEVELOPMENT,
 }

 internal enum eBuildType
 {
     CLIENT,
     HEADLESS,
 }
```

## MenuItem
 ![image](https://user-images.githubusercontent.com/79823287/122322590-a95a2780-cf60-11eb-8116-bb1efc103fed.png)
