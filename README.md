
## 函式庫介紹
MatsurikaG.dll 函式庫(以下簡稱本庫) 為專輔助於`處理`、`演算`平台開發之類別函式庫，
將常見的一維訊號處理方法彙整並經由類別宣告的形式，簡單實作在C#開發軟體及平台之中。

## 函式庫特色
* 純 C# 開發
* 於C#專案中加入參考(Reference)後即可使用(需搭配.NET 4.0、win10以前的電腦可於微軟官網下載)
* 泛用於一維訊號(聲波、電波、光譜、質譜等等)的分析、分類、建模

## 使用類別介紹
ANN使用類別分成兩部分
1. NNlayers 
2. NeualNetwork

### NNlayers
在使用一開始，我們要先建構自己的神經網路架構。
利用`NNlayers`類別，我們可先嘗試宣告單一層神經網路，舉例宣告如下

```
NNlayers layer1 = new NNlayers(NNlayers.Layers_family.Affine, Num_Input, Num_Output);
```

`Layers_family`是我們在本庫預設好的一群列舉，可在其中選擇一項當作該層屬性，
後兩項參數 `Num_Input` 、 `Num_Output` 為此層的輸入與輸出神經元數量。
可經由反覆宣告多層的`NNlayers`，並以`Array`包住，即可完成一份簡單的直線狀網路結構。
舉例如下:

     List<NNlayers> Nlist = new List<NNlayers>();
     NNlayers N1 = new NNlayers(NNlayers.Layers_family.Affine, Num_Input, numHidden);
     NNlayers N2 = new NNlayers(NNlayers.Layers_family.BN, numHidden, numHidden);
     NNlayers N3 = new NNlayers(NNlayers.Layers_family.ReLU, numHidden, numHidden);
     NNlayers N4 = new NNlayers(NNlayers.Layers_family.Affine, numHidden, Num_Output);
           
     Nlist.Add(N1);
     Nlist.Add(N2);
     Nlist.Add(N3);
     Nlist.Add(N4);
     
     NNlayers[] ANNarray = Nlist.toArray();
            

### NeualNetwork
此類別為主要學習核心，將上述的`ANNarray`輸入後即可完成一份完整的ANN學習單位，並透過調整參數、匯入數據來完成以下工作
1. Train Model
2. Improve Model

兩種宣告過程不同

#### Train Model

在一開始，我們必須呼叫出一個學習核心，該核心內要輸入上一步驟所建立的`ANNarray`。
以及輸入及輸出的維度。

    Ann = new ArtificialNeuralNetwork(ANNarray, Num_iutput, Num_output);
    
之後開始調整參數

```
Ann.PositiveLimit = 0.7;//default = 0.7
Ann.Batchratio = 1; // default = 0.2

```

調整完後即可輸入數據執行訓練。
數據的輸入是將各筆的輸入輸出合併成一個1D array，輸出放在輸入後方。

    double[][] Data = new double[data_size][];
    for(int i = 0 ; i < data_size ; i++)
    {
          Data[i] = new double[Num_input + Num_output];
          Array.Copy(Input[i], Data[i], Num_input);
          Array.Copy(Output[i], 0 , Data[i], Num_input, Num_output);
    }
    Ann.TrainModel(Data, maxEpochs, learnRate, 0);

#### Improve Model

優化模式底下我們使用前必須確認有"待優化模型"以供優化，此模型必須是本庫輸出之模型。

一開始我們已多型去宣告一個無NN架構的運算核心:

    Ann = new ArtificialNeuralNetwork( input, output);
      
接著我們將"待優化模型"的路徑記錄下，並讓DLL自動去擷取這個模型的內容。

    Ann.ImportOldProject(old_project_path);

之後就和上步驟類似，不同在最後要優化所使用的函式。
這邊我們為了讓使用者可以在優化過程接受更多的輸出結果，讓建模更有彈性。

    Ann.ImproveModel(Data, maxEpochs, learnRate, 0);

### Save Model by manual

訓練或是優化完後，如果想將此次模型保留下來，可將訓練好的模型進行儲存成一套計畫
儲存在自訂義的資料夾 `project_path`

```
Ann.Save_network(project_path, learnRate);
Ann.Save_H5files(project_path);
```
儲存成功後會在儲存目錄底下發現一份架構檔以及參數檔。兩者盡量放在同一個資料夾底下以免跟其他模型發生衝突。
詳細的代碼可參考資料夾中的cs檔。    

### Compute test data

當訓練好後、我們可以開始嘗試加入一些新數據來看一下模型的效果。
首先，與Improve Model相同的手法，匯入一套運算模型核心。或是你要使用當下已經訓練好的核心也可以。

    //匯入新模型
    Ann = new ArtificialNeuralNetwork( input, output);
    Ann.ImportOldProject(old_project_path);


之後直接將測試數據當作input進入計算。

    double[] output = Ann.Compute(input);




## 使用注意
1. 目前不支援二維圖像的資料格式。詳細的輸入格式可參考附件csv檔或是程式代碼
2. 存檔後將產生一個參數檔(.h5以及)和架構檔(.ini)，請勿分開儲存避免與其他模型混淆
3. Improve Model可接受多於原始模型的輸出層，但請將多的輸出層資料插入在陣列的最後端，例如

        Data array [input{x1, x2, x3...,xn}, ouput{y1, y2, y3, ...ym}, new output {ym+1, ym+2, ....ym+l}]
    
4. 承第三點，Improve Model不可接受多於原始模型的輸入層數量。
5. 如果不使用函式庫提供的存檔功能，可手動逐層提取參數再存成自己想要的格式。




