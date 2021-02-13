# Unity-Barracuda-TinyYoloV2-WebGL-Sample
Unity Barracudaを用いてTinyYoloV2をWebGL上で推論するサンプルです。<br>
現時点(2021/02/13)でUnityのWebGLはCPU推論のみのサポートです。<br>
GPU推論と比較しパフォーマンスは出ません。<br>
![v9g8l-5fqfl](https://user-images.githubusercontent.com/37477845/107378942-d1a7c480-6b2f-11eb-9e4f-ff17a466685e.gif)

# Demo
動作確認用ページは以下。<br>
[https://kazuhito00.github.io/Unity-Barracuda-TinyYoloV2-WebGL-Sample/WebGL-Build](https://kazuhito00.github.io/Unity-Barracuda-TinyYoloV2-WebGL-Sample/WebGL-Build/index.html)

# FPS(参考値)
WebCamController.cs の Update()の呼び出し周期を計測したものです。<br>
以下のように動作は基本的に非同期処理のため、FPSは見かけ上のFPSであり、推論自体のFPSではありません。<br>
　CSharpBurst：非同期<br>
　CSharpRef：同期<br>
　ComputePrecompiled：非同期
|  | TinyYoloV2 |
| - | :- |
| WebGL<br>CPU：Core i7-8750H CPU @2.20GHz | 約0.4FPS<br>CSharpBurst |
| Android<br>Google Pixel4a(Snapdragon 730G) | 未計測 | 未計測 |
| Unity Editor<br>GPU：GTX 1050 Ti Max-Q(4GB) | 約27FPS<br>ComputePrecompiled |

# Requirement (Unity)
* Unity 2020.1.6f1 or later
* Barracuda 1.0.4

# Reference
* [https://github.com/wojciechp6/YOLO-UnityBarracuda](https://github.com/wojciechp6/YOLO-UnityBarracuda)

# Author
高橋かずひと(https://twitter.com/KzhtTkhs)
 
# License 
Unity-Barracuda-TinyYoloV2-WebGL-Sample is under [MIT License](LICENSE).
