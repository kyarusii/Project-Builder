# Project-Builder

벤치마크를 위해 여러 옵션으로 빌드를 수행해주는 간단한 도구입니다.  
퍼포먼스 벤치마크 등을 위해 동일한 스크립트 기반의 여러 버전 빌드가 필요할 때 사용합니다.  

### Install

```json
{
    "dependencies": {
        "kr.seonghwan.project-builder": "1.1.0"
    }
}
```

```json
{
    "scopedRegistries": [
        {
            "name": "npm-seonghwan",
            "url": "https://registry.npmjs.org",
            "scopes": [
                "kr.seonghwan"
            ]
        }
    ]
}
```





### API

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

 ### MenuItem
 ![image](https://user-images.githubusercontent.com/79823287/122322590-a95a2780-cf60-11eb-8116-bb1efc103fed.png)
