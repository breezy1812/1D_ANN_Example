
## 函式庫介紹
MatsurikaG.dll 函式庫(以下簡稱本庫) 為專輔助於處理演算平台開發之類別函式庫，
將常見的一維訊號處理方法彙整並經由類別宣告的形式，簡單實現在軟體及應用程式之中。

## 函式庫特色
* C# 開發
* 加入參考(Reference)後即可使用
* 僅用於一維訊號(聲波、電波、光譜、質譜等等)

## 使用 ANN 類別
ANN使用類別分成兩部分
1. NNlayers 
2. NeualNetwork

### NNlayers
此類別可宣告單一層神經網路，宣告如下

```
NNlayers layer1 = new NNlayers(NNlayers.Layers_family.Affine, input1, output1;
```

Layers_family可選擇這層的特性，後兩項參數選擇此層的輸入與輸出神經元數量。
可經由反覆宣告多層的NNlayers，並以Array包住，即可完成一份簡單的直線狀網路結構。

### NeualNetwork
此類別為主要學習核心，將上述的NNlayers Array輸入後即可完成一份完整的ANN學習單位，並透過調整參數、匯入數據來完成以下工作
1. Train Model
2. Improve Model

兩種宣告過程不同

#### Train Model
使用前必須先將數據修改成特定格式，格式如附件.csv檔以及.cs檔中的程式敘述
```
Ann = new ArtificialNeuralNetwork(NNlayers Array, input, output);
Ann.TrainModel(Data, maxEpochs, learnRate, 0);
```

#### Improve Model
使用前必須確認有"原始模型"以供優化，此模型必須是本函式庫輸出之模型。
```
Ann = new ArtificialNeuralNetwork( input, output);
Ann.ImportOldProject(old_project_path);
Ann.ImproveModel(Data, maxEpochs, learnRate, 0);
```

以上為兩種主要功能，但其中要進行Improve Model之前，必須先將上次train好的模型進行儲存
儲存的方式為

```
Ann.Save_network(project_path, learnRate);
Ann.Save_H5files(project_path);
```
詳細的代碼可參考資料夾中的cs檔。

## 使用注意
1. 目前不支援二維圖像的資料格式。詳細的輸入格式可參考附件csv檔或是程式代碼
2. 存檔後將產生一個參數檔(.h5以及)和結構檔(.ini)，請勿分開儲存避免混淆
3. Improve Model可接受多於原始模型的輸出層，但請將多的輸出層資料插入在陣列的最後端
4. 承第三點，Improve Model不可接受多於原始模型的輸入層數量。
5. 如果不使用函式庫提供的存檔功能，可手動逐層提取參數再存承自己想要的格式。




