# ContextMatic插件

编码代理产生api幻觉。APIMatic Context为他们提供了精心策划的、版本化的API和SDK文档。

当开发人员要求代理“集成支付API”时，代理通常会根据过时的训练数据或与实际SDK不匹配的通用模式进行猜测。ContextMatic解决了这个问题，它在需要的时候为代理提供权威的、版本感知的、sdk原生的上下文。

它包括什么

MCP服务器

|服务器|描述|| --------------- | ---------------------------------------------------------------------------------- |
|`context-matic`|托管MCP服务器，用于版本感知第三方API集成和SDK发现。|

# # #技能

|技能|描述|| -------------------------- | ------------------------------------------------------------------------------------------------------------- |
|`/integrate-context-matic`|使用权威SDK和端点信息集成受支持的第三方api的集中工作流。|
|`/onboard-context-matic`|介绍ContextMatic MCP服务器、支持的api和工具使用。|

ContextMatic做什么

ContextMatic提供了基于真实API定义和SDK的GitHub Copilot版本感知API和SDK指导，而不是通用的公共示例。它有助于：

-按项目语言发现API
—认证和快速入门指导
-带有参数和响应细节的端点查找
-使用类型化属性定义进行模型查找

##支持的api

该插件为以下api提供代理sdk原生上下文，可在TypeScript、c#、Python、Java、PHP和Ruby中使用：

|接口|描述|| ------------------------------ | ----------------------------------------------------------------------------------------- |
| **Adyen API** |支付处理：检索支付方法，创建订单，管理存储的支付令牌|
| ** |位置服务：地理编码、方向、距离矩阵、海拔、道路和地点|
| **PayPal服务器SDK** |支付流程：订单、支付、保险库、交易搜索、订阅|
** payfaster API** |支付和金融服务：项目协议、银行账户、回款报价|
| **Slack API** |工作空间自动化：OAuth bots，消息传递，会话管理|
|音乐和播客：库管理，播放控制，发现|
| **特斯拉车队管理API** |车辆和车队运营：充电历史、车辆命令、能源管理|
| * * Tesser数字支付：支付意向、链上支付、应用管理|
| **Twilio API** |通讯：短信、语音、视频、验证服务|这个名单还在不断增加。[建议一个新的API]（# contribute）请求对这里没有列出的API的支持。

---

插件给代理什么

安装后，该插件将向代理公开七个工具。每个工具都映射到集成工作流的特定阶段：

| Tool |开发人员任务，启用|| ----------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
|`fetch_api`|提供精确的API匹配或列出所提供的`language`的所有可用API，包括每个API的名称、密钥和描述。传递项目的语言和API`key`进行精确匹配查找（只返回该API）。如果没有找到精确匹配，则返回该`language`的完整API目录。代理首先调用它来发现哪些api可用。|
与API Copilot聊天，了解逐步集成指导和一般API问题：身份验证设置、客户端初始化、功能行为、特定于框架的模式(例如；“如何在Laravel中初始化Twilio客户端？”)，以及惯用的SDK代码示例。|
|`endpoint_search`|根据方法名返回SDK端点方法的描述、输入参数和响应形状。|
|`model_search`|按名称返回SDK模型的完整定义及其类型化属性。在编写构造请求主体或读取响应对象的代码之前调用这个函数。|
|`update_activity`|记录具体的集成里程碑，如SDK设置、认证配置、第一个成功的API调用和解决的错误。代理在代码或基础结构中实际达到某个里程碑后调用此方法。|
|`add_guidelines`|添加代理在实现期间可以遵循的特定于语言的指导文件，如安全性、测试或工作流指导。|
|`add_skills`|添加可重用的项目技能，如`{language}-conventions`，以便未来的API集成工作可以遵循项目特定于语言的约定。|要获得如何同时使用这些工具的分步指导，请在代理中调用`/integrate-context-matic`技能。它告诉代理在整个集成工作流中何时以及如何调用每个工具。

---

##从提示到代码：工具如何协同工作

这七个工具被设计成在一个自然的集成工作流中链接在一起。下面是一个具体的例子，当agent接收到一个真正的任务时，会发生什么：

**你的提示：** _“/ integrated - contex- matic添加Twilio短信通知到我的Next.js应用程序。发送文本时，一个订单发货。

|步骤|工具称为|它返回|| ---- | --------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 1 |`add_guidelines`(`language=typescript`) |添加代理在开始API集成之前可以遵循的安全、测试和实现工作流的项目指导文件。|
|`add_skills`(`language=typescript`) |添加可重用的特定于语言的技能，如约定指导，以便项目设置与未来的集成工作相匹配。|
| |`fetch_api`(`language=typescript`,`key="twilio"`) |找到精确匹配-返回Twilio的条目，包含其名称，键和描述|`key=twilio`, query=_“如何初始化Twilio TypeScript客户端？”_) |返回带有验证配置的准确SDK设置代码|
|`update_activity`(`milestone=auth_configured`) |返回的SDK/auth配置已添加到应用程序后，记录凭证已连接到应用程序，并且集成已准备好进行第一次实时调用|
| 6 |`endpoint_search`(`query=createMessage`) |返回短信发送端点|的方法签名、所需参数和认证要求
bbb7 |`model_search`(`query=CreateMessageRequest`) |返回所有可用字段|的完整类型请求模型
bbb8 |`ask`(`query="How do I handle delivery status callbacks in Next.js?"`) |返回与Twilio对齐的webhook处理代码SDK                                                                                                                    |每个步骤在单个工具调用中完成。代理处理业务流程。你描述了目标，它就会在正确的时间选择正确的工具。

MCP服务器

这个插件使用ContextMatic MCP端点：```text
https://chatbotapi.apimatic.io/mcp/plugins
```
插件通过其插件根`.mcp.json`文件注册MCP服务器，因此服务器与捆绑的技能一起可用。

---

##在几分钟内构建一个完整的应用程序<details>
<summary><strong>PayPal Instant Storefront — Node.js/Express · 30 min</strong></summary>
！(贝宝)(https://img.shields.io/badge/-PayPal-003087?logo=paypal&logoColor=white&labelColor=003087) ![Node.js] (https://img.shields.io/badge/-Node.js-339933?logo=nodedotjs&logoColor=white&labelColor=339933) !(表达)(https://img.shields.io/badge/-Express-000000?logo=express&logoColor=white&labelColor=000000) ![JavaScript] (https://img.shields.io/badge/-JavaScript-F7DF1E?logo=javascript&logoColor=black&labelColor=F7DF1E)

！[paypalsampleapp] (https://github.com/user-attachments/assets/dc3e5b02-934e-44b5-9df9-20387557babe)

**构建内容：**一个完整的Node.js/Express店面，具有产品管理，每个产品的可共享结帐链接，PayPal智能支付按钮，服务器端订单创建和捕获，以及支付历史仪表板。

* *提示:* *```
/integrate-context-matic Build me a "PayPal Instant Storefront" app.
The app has a setup page where I enter my PayPal client-id and secret once, then
a product creation form where I enter a product name, description, price, currency,
and upload or provide product images. When I click "Generate Checkout Page" it
creates a live, shareable checkout URL like /checkout/abc123 that anyone can open —
they see the product details with images, price, description, and a working PayPal
Smart Payment Button. The payment flow should be fully server-side using the PayPal
Server SDK: backend creates the order when buyer clicks pay, captures it after
approval, and shows a confirmation page with order details. I should be able to
create multiple products and each gets its own unique checkout link I can share
with anyone. Include a simple dashboard where I can see all my products and their
checkout links, plus a list of completed payments showing order ID, buyer info,
amount, and status for each product. The checkout pages should be mobile-responsive
and look like real professional product pages. Support sandbox and live mode via
environment variables. Only use the Orders API and Payments API, do not use
Transaction Search or Vault. Make it deployable with npm install and npm start.
```
**工具如何使用：**

|步骤|工具|查询|返回的内容|| ---- | ----------------- | --------------------------------- | -------------------------------------------------------------------------------------------------- |
| 1 |`fetch_api`|`language=typescript`|可用接口；使用密钥`paypal`|识别PayPal服务器SDK
|客户端初始化代码，`.env`结构，沙箱vs.通过`Client.fromEnvironment`|进行实时配置
| |`ask`|订单创建流程|端到端创建→批准→捕获流程，包含完整的TypeScript服务器端代码|
|`endpoint_search`|`ordersCreate`|`CreateOrder`方法签名，`OrderRequest`主体结构，响应类型`Order`，错误码|
bbb5 |`endpoint_search`|`capture`|`CaptureOrder`合约-必需的`id`参数，可选主体，捕获ID位置在响应|
| 6 |`model_search`|`OrderRequest`|全请求模型属性；标记`payer`和`application_context`为已弃用|bbb7 |`model_search`|`Money`|结构性金额的货币代码和值字段|
bbb8 |`ask`|智能支付按钮|前端按钮集成-`createOrder`/`onApprove`布线到后端端点|
bbb9 |`endpoint_search`|`getOrder`|`GetOrder`方法签名和确认页|的响应形状
|0 |`model_search`|`PurchaseUnitRequest`|全量型号，包含`amount`，`items`,`shipping`，以及所有可选字段|
|1 |`model_search`|`Order`|全响应模型-`status`，`purchaseUnits`,`links`(含`approve`重定向URL* *应用结果:* *

-一次性凭据设置页面与实时沙箱验证
-产品创建与名称，描述，价格，货币和图片上传
-每个产品唯一可共享的结帐URL （`/checkout/abc123`）
-服务器端订单创建和捕获-没有客户端机密暴露
-包含订单ID、买家信息和捕获细节的确认页面
-所有产品，总收入和付款历史的仪表板
-移动响应的结账页面
-可部署`npm install && npm start`**构建时间：**生成10分钟+测试20分钟= **总共30分钟**</details>

<details>
<summary><strong>Spotify Music DNA Card — Python/Flask · 30 min</strong></summary>
！(Spotify) (https://img.shields.io/badge/-Spotify-1DB954?logo=spotify&logoColor=white&labelColor=1DB954) !(Python) (https://img.shields.io/badge/-Python-3776AB?logo=python&logoColor=white&labelColor=3776AB) !(瓶)(https://img.shields.io/badge/-Flask-000000?logo=flask&logoColor=white&labelColor=000000)

！[spotifySampleApp] (https://github.com/user-attachments/assets/63556c36-ba2d-417c-978c-5a4697e9b4e2)

**构建内容：**一个Python/Flask网络应用程序，用户通过Spotify OAuth进行身份验证，获取他们的顶级艺术家和曲目，批量检索音频功能，并分析数据以产生个性化的“音乐DNA”卡-具有平均音频功能的雷达图，前5个流派，最晦涩的艺术家和生成的个性标签-与download/share按钮。仅自定义品牌；没有Spotify的标志。

* *提示:* *```
/integrate-context-matic Create a web app using Python where users log in with Spotify,
fetch their top artists and top tracks, then fetch audio features for those tracks.
Analyze the data to calculate average audio features, find the most obscure artist,
determine the top 5 genres, and generate a "music personality" label based on the
averages. Render all of this in a visually appealing DNA card with a radar chart,
top genres, most obscure artist, and personality label, and include a button to
download or share the card. Use your own branding and logo; do not include Spotify
logos anywhere.
```
**工具如何使用：**

|步骤|工具|查询|返回的内容|| ---- | ----------------- | ----------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 1 |`fetch_api`|`language=python`|可用接口；使用密钥`spotify`|识别Spotify Web API SDK
|`ask`| SDK设置，用户登录OAuth 2.0授权码流程|完整`pip install spotify-api-sdk`设置，`SpotifywebapiClient`初始化采用`AuthorizationCodeAuthCredentials`，`.env`结构，`get_authorization_url()`→`fetch_token(code)`→`clone_with(o_auth_token=token)`流程，令牌刷新模式|
|`ask`|如何获取用户的顶级艺术家和顶级曲目|端到端代码使用`users_controller.get_users_top_artists()`和`users_controller.get_users_top_tracks()`与`time_range`，`limit`，`offset`参数；读取`PagingArtistObject.items`和`PagingTrackObject.items`|
bbb4 |`endpoint_search`|`get_users_top_artists`|方法签名E -参数`time_range`，`limit`,`offset`；响应类型`PagingArtistObject`；所需范围`OAuthScopeEnum.USER_TOP_READ`|
bbb5 |`endpoint_search`|`get_users_top_tracks`|方法签名-与顶级艺术家相同的参数；响应类型`PagingTrackObject`，带有`List[TrackObject]`项|
| 6 |`endpoint_search`|`get_audio_features`|单轨法经`tracks_controller.get_audio_features(id)`；响应类型为`AudioFeaturesObject`|
bbb7 |`endpoint_search`|`get_several_audio_features`|批处理方法通过`tracks_controller.get_several_audio_features(ids)`-带逗号分隔额定轨道id字符串；响应类型为`ManyAudioFeatures`|
bbb8 |`endpoint_search`|`get_current_users_profile`|`users_controller.get_current_users_profile()`-无参数；响应`PrivateUserObject`;所需范围`USER_READ_EMAIL`，`USER_READ_PRIVATE`|
bbb9 |`model_search`|`AudioFeaturesObject`|所有14个属性-`danceability`，`energy`,`valence`,`acousticness`,`instrumentalness`,`liveness`,`speechiness`,`tempo`,`loudness`,`key`,`mode`,`time_signature`,`duration_ms`,`uri`(所有0.0-1.0浮动用于雷达图和个性逻辑
|0 |`model_search`|`ArtistObject`|属性`name`，`id`,`popularity`（0-100 int，用于寻找最晦涩的艺术家），`genres`(`List[str]`，使用对于前5名类型聚合),`images`,`external_urls`|
|1 |`model_search`|`TrackObject`|属性`id`（需要音频特性批调用），`name`,`popularity`,`artists`(`List[ArtistObject]`),`album`,`duration_ms`,`uri`|
|2 |`model_search`|`PagingTrackObject`|分页包装-`items`(`List[TrackObject]`),`total`,`next`,`offset`,`limit`|
| 13 |`model_search`|`ManyAudioFeatures`|批响应封装器-`audio_features`（`List[AudioFeaturesObject]`），用于迭代和平均|
|4 |`model_search`|`PrivateUserObject`|用户配置文件-`display_name`，`images`(`List[ImageObject]`),`id`,`email`,`country`(用于个性化DNA卡头* *应用结果:* *- Spotify OAuth 2.0通过授权码流登录（没有客户端机密暴露给浏览器）
-获取当前用户的配置文件（`display_name`，头像）个性化卡
-检索前50名艺术家和前50名曲目（可配置`time_range`:short/medium/longterm）
-批量获取音频功能的所有顶级轨道通过`get_several_audio_features`-计算所有音轨的平均音频特征（舞蹈性，能量，价态，声学性，器质性，活跃性，言语性）
-识别最不知名的艺术家（在顶级艺术家中最低的`popularity`分数）
-从所有顶级艺术家的流派列表中汇总并排名前5个流派
-根据平均特征阈值生成“音乐个性”标签（例如，“Energetic Explorer”，“Melancholic dreaming”，“Chill Acoustic Soul”）
-呈现具有视觉吸引力的DNA卡：
-雷达图（Chart.js）的7个平均音频功能
-前5个带有视觉徽章的游戏类型-最不起眼的艺术家的名字和人气得分
-显著显示个性标签
—用户的显示名和头像
-下载卡为PNG和共享按钮（html2canvas）
-自定义品牌和标志贯穿-没有Spotify的标志任何地方
-长会话令牌刷新处理
-可部署`pip install -r requirements.txt && python app.py`</details>

<details>
<summary><strong>Google Maps Restaurant Roulette — PHP · 30 min</strong></summary>
！[谷歌地图](https://img.shields.io/badge/-Google%20Maps-4285F4?logo=googlemaps&logoColor=white&labelColor=4285F4) ！(PHP) (https://img.shields.io/badge/-PHP-777BB4?logo=php&logoColor=white&labelColor=777BB4) ![JavaScript] (https://img.shields.io/badge/-JavaScript-F7DF1E?logo=javascript&logoColor=black&labelColor=F7DF1E)

！[google-maps-sample-app] (https://github.com/user-attachments/assets/eafab114-ccf8-42f9-84c3-bc9706706118)

**构建内容：**一个PHP web应用程序，用户可以在谷歌地图上放置一个大头针（或使用他们的位置），绘制一个旅行半径的圆圈，然后点击“旋转”来随机选择该半径内的一家餐厅。该应用程序显示谷歌地点照片，街景店面预览，一键导航-带有轮子动画和游戏化悬念的“再次旋转”按钮。自定义品牌;凭据通过`.env`文件。

* *提示:* *```
/integrate-context-matic Create a web application using php and google maps platform
apis sdk. for credentials create an env file in which the user will provide the API
Key. The user will Drop a pin (or use your location) on the map, draw a circle for
how far you are willing to travel, and click "spin." The app picks a random restaurant
within that radius, shows you photos from Google Places, a Street View preview of the
storefront, and one-click directions. Not happy with the pick? Spin again. The wheel
animation and suspense make it feel like a game.
```
**工具如何使用：**

|步骤|工具|查询|返回的内容|| ---- | ----------------- | ----------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 1 |`fetch_api`|`language=php`|可用接口；识别谷歌地图平台SDK的关键字`googlemaps`（也：`paypal`，`spotify`,`maxio`,`verizon`） |
|`composer require sdksio/google-maps-platform-sdk:1.0.3`,`GoogleMapsPlatformClientBuilder::init()`with`CustomQueryAuthenticationCredentialsBuilder::init('key')`,`.env`structure,`Environment::PRODUCTION`|
| |`ask`|如何搜索附近的餐馆半径|完整的代码使用`$client->getPlacesApi()->nearbySearch($location, $radius, 'restaurant', ...)`，响应处理通过`isSuccess()`/`getResult()`，迭代`Place[]`结果|
bbb4 |`endpoint_search`|`nearbySearch`|方法签名参数：`location`(`"lat,lng"`),`radius`（米），`keyword`,`maxprice`,`minprice`,`opennow`,`pagetoken`,`rankby`,`type`,`language`；响应类型为`PlacesNearbySearchResponse`|
bbb5 |`endpoint_search`|`placeDetails`|方法签名-参数`placeId`、`fields[]`（Basic/Contact/Atmosphere类）、`sessiontoken`、`language`、`region`；响应类型为`PlacesDetailsResponse`|
bbb6 |`endpoint_search`|`placePhoto`|方法签名-参数`photoReference`（字符串），`maxheight`,`maxwidth`(1-1600px)；响应类型`mixed`（原始图像字节）|
bbb7 |`endpoint_search`|`streetView`|方法签名-参数`size`(`"{w}x{h}"`, max 640px),`fov`,`heading`,`location`,`pitch`,`radius`,`source`；响应typE`mixed`（图像字节）|
| 8 |`endpoint_search`|`directions`|方法签名-参数`destination`、`origin`、`mode`、`avoid`、`units`、`waypoints`、`language`、`region`；响应类型为`DirectionsResponse`|
bbb9 |`model_search`|`PlacesNearbySearchResponse`|属性：`results`(`Place[]`),`status`(`PlacesSearchStatus`),`nextPageToken`,`errorMessage`,`htmlAttributions`|
|0 |`model_search`|`PlacesDetailsResponse`|属性：`result`(`Place`),`status`(`PlacesDetailsStatus`),`htmlAttributions`,`infoMessages`|
|1 |`model_search`|`Place`|全型号-`name`，`placeId`,`formattedAddress`,`geometry`(`Geometry`),`rating`,`userRatingsTotal`,`priceLevel`,`photos`(`PlacePhoto[]`),`openingHours`,`types`,`vicinity`,`website`,`businessStatus`,`reviews`(`PlaceReview[]`) |
|2 |`model_search`|`PlacePhoto`|属性：`photoReference`（字符串，用于`placePhoto`调用），`height`,`width`,`htmlAttributions`|
| 13 |`model_search`|`Geometry`|属性：`location`(`LatLngLiteral`),`viewport`（`Bounds`）|
|4 |`model_search`|`LatLngLiteral`|属性：`lat`（浮动），`lng`（浮动）-用于提取街景和方向的坐标|
|5 |`model_search`|`DirectionsResponse`|属性：`routes`(`DirectionsRoute[]`),`status`(`DirectionsStatus`),`geocodedWaypoints`,`availableTravelModes`,`errorMessage`|
| 16 |`ask`|如何使用街景静态API给定lat/lng|`$client->getStreetViewApi()->streetView($size, null, null, $location)`，返回原始图像字节；`streetViewMetadata()`用于可用性检查|* *应用结果:* *

-`.env`文件，使用`GOOGLE_MAPS_API_KEY`作为凭据
-交互式谷歌地图，可点击放置图钉或“使用我的位置”（浏览器地理定位）
-可拖动的圆圈叠加设置移动半径（米）
-“旋转”按钮与wheel/slot-machine动画悬疑
-后端`nearbySearch`与`keyword=restaurant`绘制半径内
-从`Place[]`结果中随机选择餐厅
-场地详情卡显示：
-餐厅名称，评级，价格水平和格式化地址
-谷歌通过`placePhoto`与`photoReference`放置照片旋转木马
-街景店面预览通过`streetView`使用该地方的lat/lng-一键式路线链接（路线API或谷歌地图URL与`origin`和`destination`）
-“再次旋转”按钮重新滚动不改变pin/radius-通过`nextPageToken`分页支持更多的结果
-移动响应地图和卡片布局
-可部署`composer install && php -S localhost:8000`</details>
---

提示“尝试”

体验ContextMatic的最佳方式是在安装插件后将这些提示直接粘贴到Cursor或Claude Code中。每个提示符都是为了触发完整的工具链而编写的。<details>
<summary><strong>Quickstart: your first API call</strong></summary>
！(Spotify) (https://img.shields.io/badge/-Spotify-1DB954?logo=spotify&logoColor=white&labelColor=1DB954) !(打印稿)(https://img.shields.io/badge/-TypeScript-3178C6?logo=typescript&logoColor=white&labelColor=3178C6)```
/integrate-context-matic Set up the Spotify TypeScript SDK and fetch my top 5 tracks.
Show me the complete client initialization and the API call.
```
---

！(为什么Twilio) (https://img.shields.io/badge/-Twilio-F22F46?logo=twilio&logoColor=white&labelColor=F22F46) !(PHP) (https://img.shields.io/badge/-PHP-777BB4?logo=php&logoColor=white&labelColor=777BB4)```
/integrate-context-matic How do I authenticate with the Twilio API and send an SMS?
Give me the full PHP setup including the SDK client and the send call.
```
---

！(松弛)(https://img.shields.io/badge/-Slack-4A154B?logo=slack&logoColor=white&labelColor=4A154B) !Python (https://img.shields.io/badge/-Python-3776AB?logo=python&logoColor=white&labelColor=3776AB)```
/integrate-context-matic Walk me through initializing the Slack API client
in a Python script and posting a message to a channel.
```

</details>

<details>
<summary><strong>Framework-specific integration</strong></summary>
！[谷歌地图](https://img.shields.io/badge/-Google%20Maps-4285F4?logo=googlemaps&logoColor=white&labelColor=4285F4) ！[Next.js] (https://img.shields.io/badge/-Next.js-000000?logo=nextdotjs&logoColor=white&labelColor=000000) !(打印稿)(https://img.shields.io/badge/-TypeScript-3178C6?logo=typescript&logoColor=white&labelColor=3178C6)```
/integrate-context-matic I'm building a Next.js app. Integrate the Google Maps
Places API to search for nearby restaurants and display them on a page.
Use the TypeScript SDK.
```
---

！(为什么Twilio) (https://img.shields.io/badge/-Twilio-F22F46?logo=twilio&logoColor=white&labelColor=F22F46) ![Laravel] (https://img.shields.io/badge/-Laravel-FF2D20?logo=laravel&logoColor=white&labelColor=FF2D20) !(PHP) (https://img.shields.io/badge/-PHP-777BB4?logo=php&logoColor=white&labelColor=777BB4)```
/integrate-context-matic I'm using Laravel. Show me how to send a Twilio SMS
when a user registers. Include the PHP SDK setup, client initialization, and the
controller code.
```
---

！[为什么Twilio] (https://img.shields.io/badge/-Twilio-F22F46?logo=twilio&logoColor=white&labelColor=F22F46) ! [ASP。（https://img.shields.io/badge/-ASP.NET%20Core-512BD4?logo=dotnet&logoColor=white&labelColor=512BD4） ！(c#) (https://img.shields.io/badge/-C%23-239120?logo=csharp&logoColor=white&labelColor=239120)```
/integrate-context-matic I have an ASP.NET Core app. Add Twilio webhook handling
so I can receive delivery status callbacks when an SMS is sent.
```

</details>

<details>
<summary><strong>Chaining tools for full integrations</strong></summary>
这些提示被设计用来练习完整的插件工作流程；从API发现到端点查找，再到生产就绪的代码。

！(为什么Twilio) (https://img.shields.io/badge/-Twilio-F22F46?logo=twilio&logoColor=white&labelColor=F22F46) ![Next.js] (https://img.shields.io/badge/-Next.js-000000?logo=nextdotjs&logoColor=white&labelColor=000000) !(打印稿)(https://img.shields.io/badge/-TypeScript-3178C6?logo=typescript&logoColor=white&labelColor=3178C6)```
/integrate-context-matic I want to add real-time order shipping notifications
to my Next.js store. Use Twilio to send an SMS when the order status changes to
"shipped". Show me the full integration: SDK setup, the correct endpoint and its
parameters, and the TypeScript code.
```
---

！(松弛)(https://img.shields.io/badge/-Slack-4A154B?logo=slack&logoColor=white&labelColor=4A154B) !(Spotify) (https://img.shields.io/badge/-Spotify-1DB954?logo=spotify&logoColor=white&labelColor=1DB954) !(打印稿)(https://img.shields.io/badge/-TypeScript-3178C6?logo=typescript&logoColor=white&labelColor=3178C6)```
/integrate-context-matic I need to post a Slack message every time a Spotify
track changes in my playlist monitoring app. Walk me through integrating both APIs
in TypeScript — start by discovering what's available, then show me the auth setup
and the exact API calls.
```
---

！[谷歌地图](https://img.shields.io/badge/-Google%20Maps-4285F4?logo=googlemaps&logoColor=white&labelColor=4285F4) ！（https://img.shields.io/badge/-ASP.NET%20Core-512BD4?logo=dotnet&logoColor=white&labelColor=512BD4） ！(c#) (https://img.shields.io/badge/-C%23-239120?logo=csharp&logoColor=white&labelColor=239120)```
/integrate-context-matic In my ASP.NET Core app, I want to geocode user
addresses using Google Maps and cache the results. Look up the geocode endpoint
and response model, then generate the C# code including error handling.
```

</details>

<details>
<summary><strong>Debugging and error handling</strong></summary>
！(Spotify) (https://img.shields.io/badge/-Spotify-1DB954?logo=spotify&logoColor=white&labelColor=1DB954) !(打印稿)(https://img.shields.io/badge/-TypeScript-3178C6?logo=typescript&logoColor=white&labelColor=3178C6)```
/integrate-context-matic My Spotify API call is returning 401. What OAuth flow
should I be using and how does the TypeScript SDK handle token refresh automatically?
```
---

！(松弛)(https://img.shields.io/badge/-Slack-4A154B?logo=slack&logoColor=white&labelColor=4A154B) !Python (https://img.shields.io/badge/-Python-3776AB?logo=python&logoColor=white&labelColor=3776AB)```
/integrate-context-matic My Slack message posts are failing intermittently
with rate limit errors. How does the Python SDK expose rate limit information and
what's the recommended retry pattern?
```

</details>
---

典型用例

发现TypeScript、Python、Java、PHP、Ruby、Go或c#项目中支持的api
-以项目语言获取受支持的第三方API的分步集成指导
—使用版本感知SDK指导设置身份验证、客户端初始化和第一个API调用
-根据SDK类型编写代码前检查请求和响应模型
-查找实现过程中所需的确切方法、参数和响应类型

APIMatic如何为API生成上下文

！[API集成使用ContextMatic]（https://github.com/apimatic/context-matic/blob/dev/assets/images/image.png?raw=true）

APIMatic通过相同的SDK生成管道获取OpenAPI规范，该管道用于生成10多种语言的习惯的、类型安全的SDK。由此产生的MCP服务器将SDK文档和集成模式公开为结构化的工具响应，AI助手可以本地使用这些响应。这意味着AI接收到的情境是：

-源自实际生成的SDK代码，而不是原始文档
-包括惯用模式、类型化模型和错误处理
-与当前版本的API规范保持一致

对于API提供者：[request a demo]（https://www.apimatic.io/request-demo）为你的API生成上下文。

# #源

这个插件贡献改编自APIMatic ContextMatic项目，并打包为Awesome Copilot。

GitHub来源:- https://github.com/apimatic/context-matic
# #贡献

有请求或发现问题？使用以下模板之一：

[请求一种新语言](https://github.com/apimatic/context-matic/issues/new?template=language-request.yml) -请求支持一种新的SDK语言（例如Swift， Kotlin, Rust）
- [Request a new API](https://github.com/apimatic/context-matic/issues/new?template=api-request.yml) -请求一个新的第三方API被添加到目录中
—[报告问题或给出反馈]（https://github.com/apimatic/context-matic/issues/new?template=issue-feedback.yml）—报告错误，分享反馈，或对现有工具提出改进建议

关于其他任何问题，请[打开空白问题]（https://github.com/apimatic/context-matic/issues/new）或联系[support@apimatic.io]（mailto:support@apimatic.io）。

##了解更多

-[产品页面]（https://www.apimatic.io/product/context-plugins）
-[博客：从API门户到光标]（https://www.apimatic.io/blog/from-api-portals-to-cursor）
-[案例分析]（https://www.apimatic.io/product/context-plugins/case-study）

---

# #许可证

麻省理工学院