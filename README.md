# KirkRendererPipeline
KirkRendererPipeline 自定义渲染管线练习
# 介绍

本管线主要用于自己学习和研究，收录、改写以及自己创作的一些效果和图形技术。

---

## 第零部分 KirkRendererPipeline管线
 *未开启GI是前向渲染，开启GI后是延迟渲染，支持PPV2后处理，SRP Batching。*·
- INCLUDE：
  - 1.KirkRendererAsset.cs
  - 2.KirkRendererPipeline.cs
  - 3.GIRenderer.cs

 **截图**：
![Image 1](https://github.com/Kirkice/SRPTest/blob/main/SourceImages/7.png?raw=true)

![Image 2](https://github.com/Kirkice/SRPTest/tree/main/SourceImages/8.png?raw=true)

## 第一部分 基础光照模型

 *共包含5种简单光照模型，同时支持四盏平行光、四盏点光源、四盏聚光灯。*·

**LIGHT MODEL**：
- INCLUDE：
  - 1.Lambert
  - 2.HalfLambert
  - 3.Phong
  - 4.BlinnPhong
  - 5.CookTorrance
  
 **截图**：
![Image 3](https://github.com/Kirkice/SRPTest/tree/main/SourceImages/lightModel.png?raw=true)

---

## 第二部分 PBR材质 KirkLit
 *共包含BRDF/BSDF两种PBR光照，同时支持四盏平行光、四盏点光源、四盏聚光灯，支持金属度、粗糙度、法线、AO贴图*·
**Kirk Lit**：
- INCLUDE：
  - 1.BRDF
  - 2.BSDF

  
 **截图**：
 - 着色器参数
 ![Image 4](https://github.com/Kirkice/SRPTest/tree/main/SourceImages/pbrM.png?raw=true)
 - PBR效果
 ![Image 5](https://github.com/Kirkice/SRPTest/tree/main/SourceImages/PBRLit.png?raw=true)

---

## 第三部分 全局光照 Global illumination

**Global illumination**：
- INCLUDE：
  - 1.Voxel GI (体素化GI)
  - 2 Ray Tracing (光线追踪)

**截图**：
 - Global illumination OFF
 ![Image 6](https://github.com/Kirkice/SRPTest/tree/main/SourceImages/GIOff.png?raw=true)
 - Global illumination ON！！
 ![Image 7](https://github.com/Kirkice/SRPTest/tree/main/SourceImages/GIOn.png?raw=true)
 
---

## 第四部分 体渲染 Ray Marching / Voxel

- INCLUDE：
  - 1.Volume Light (体积光)
  - 2 Volume Cloud (体积云)
  - 3 Volume Fog   (体积雾)
  - 4 Voxel   (体素化)

**截图**：

 - Volume Light
 ![Image 8](https://github.com/Kirkice/SRPTest/tree/main/SourceImages/Point.png?raw=true)
 ![Image 9](https://github.com/Kirkice/SRPTest/tree/main/SourceImages/Spot.png?raw=true)

 - Volume Cloud / Voxel
   还在写。。。。
   
 - Volume Fog 
 ![Image 10](https://github.com/Kirkice/SRPTest/tree/main/SourceImages/DirectionLight.png?raw=true)

 ---

## 第四部分 后处理 Kirk - PostProcessing
- INCLUDE：
  - 1.Bokeh Blur (散景模糊)
  - 2.Brightness And Contrast (亮度对比度)
  - 3.Bloom (泛光)
  - 4.HSV (色相、饱和度、明度)
  - 5.Mosaic (马赛克)
  - 6.RGB Split (RGB分离)
  - 7.Radial Blur (径向模糊)
  - 8.Robert Edge (Robert算子描边)
  - 9.Turn Gary (去色)
  - 10.Voronoi (泰森多边形)
  - 11.Broken (屏幕破碎)
  - 12.Box Blur (均值模糊)
  - 13.White Balance (白平衡)
  - 14.Sharpen (锐化)
  - 15.Vignette (边缘遮罩)
  - 16.Oil Paint (油画笔触)
  - 17.Spherize (球面化)
  - 18.Relief (浮雕)
  - 19.Explosure (曝光度)
  - 20.Triangle (三角填充)
  - 21.ColorSpace (色彩空间)
  - 22.GlobalFog (全局雾效)
  - 23.MotionBlur (运动模糊)
  - 24.TAA (TAA抗锯齿)
  - 25.ScreenSpaceAmbitionOcclusion (屏幕空间环境光遮蔽)
- 还没写的 0.0:
  - 26.ScreenSpaceReflection (屏幕空间反射)    
  - 27.ToneMapping (色调映射)  

**截图**：
  ![Image 11](https://github.com/Kirkice/SRPTest/tree/main/SourceImages/1.png?raw=true)

  ![Image 12](https://github.com/Kirkice/SRPTest/tree/main/SourceImages/2.png?raw=true)

  ![Image 13](https://github.com/Kirkice/SRPTest/tree/main/SourceImages/3.png?raw=true)

  ![Image 14](https://github.com/Kirkice/SRPTest/tree/main/SourceImages/4.png?raw=true)

  ![Image 15](https://github.com/Kirkice/SRPTest/tree/main/SourceImages/5.png?raw=true)

  ![Image 16](https://github.com/Kirkice/SRPTest/tree/main/SourceImages/6.png?raw=true)
