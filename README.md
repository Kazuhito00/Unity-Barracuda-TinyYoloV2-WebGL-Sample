# Unity-Barracuda-TinyYoloV2-WebGL-Sample
Unity Barracudaを用いてTinyYoloV2をWebGL上で推論するサンプルです。<br>
現時点(2021/02/13)でUnityのWebGLはCPU推論のみのサポートです。<br>
GPU推論と比較しパフォーマンスは出ません。<br>
■Unity Editor実行<br>
![zppnt-jlek8](https://user-images.githubusercontent.com/37477845/107845474-fbf2ce00-6e1e-11eb-9b0a-00a80717e779.gif)
■WebGL実行<br>
![2wrie-hauk1](https://user-images.githubusercontent.com/37477845/107845479-04e39f80-6e1f-11eb-84cf-8bead776db48.gif)

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
