# TaskPool

覚えるものは「TaskPool」「Task」「Agent」の3つです。
TaskPoolとは、発生する複数のTaskをAgentによって処理の開始から終了までを扱うデザインパターンです。

<!-- vim-markdown-toc GFM -->

* [主な機能](#主な機能)
* [ファイル構成](#ファイル構成)
* [簡単なサンプル](#簡単なサンプル)
    * [Taskを作成する](#taskを作成する)
    * [Taskを処理するAgentを作成する](#taskを処理するagentを作成する)
    * [TaskPoolを準備する](#taskpoolを準備する)
    * [TaskPoolを使ってTaskを動かしてみる](#taskpoolを使ってtaskを動かしてみる)

<!-- vim-markdown-toc -->

## 主な機能

* Taskをプールさせることができる。
* TaskにはPriorityとして優先度を加えることができる。
* Agent数を決めることで、同時に処理するTask数を制限できる。
    * 例) 同時に通信する数を2つに制限したい `Agent = 2`

## ファイル構成

```
./
├── ITaskAgent.cs       --- Agentのインターフェイス。Agentを実装するには、このインターフェイスから実装します
├── README.md           --- このファイルです
├── StartTaskStatus.cs  --- Taskの開始を表すステータス定義
├── TaskBase.cs         --- Taskの基底クラス。Taskを実装するには、このクラスを継承します
├── TaskInformation.cs  --- Taskの情報。直接使うことはありません。
├── TaskPool.cs         --- Taskプールの本体。このクラスでTaskを集約して管理します
└── TaskStatus.cs       --- Taskのステータス定義
```

## 簡単なサンプル

### Taskを作成する

```csharp
sealed class LoadAssetTask : TaskBase {
    public string AssetName { get; private set; }
    public DateTime StartTime { get; set; }

    public static LoadAssetTask Create(string assetName) {
        var loadAssetTask = ReferencePool.Acquire<LoadAssetTask>();
        loadAssetTask.AssetName = assetName;
        return loadAssetTask;
    }
}
```

* Taskの立ち位置は、自身で処理するのではなくTask情報の責務を受け持つところです
* ここでは、以下の2つを受け持っています
    * アセット名
    * タスクの開始時間

### Taskを処理するAgentを作成する

```csharp
sealed class LoadAssetAgent : ITaskAgent<LoadAssetTask> {
    private LoadAssetTask _task;

    public LoadAssetAgent() {
        Reset();
    }

#region ITaskAgent<LoadAssetTask>
    public T Task => _task;

    public void Initialize() {
        // Agentの初期化処理を実装する
    }

    public void Shutdown() {
        // Agentの終了処理を実装する
    }

    public void Update(float elapseSeconds, float realElapseSeconds) {
        // Agentのプーリング処理を実装する
    }

    public StartTaskStatus Start(LoadAssetTask task) {
        Assert.IsNotNull(task, "Task is invalid.");

        _task = task;
        _task.StartTime = DateTime.UtcNow;

        // ロードするリソースが、既にロード中なので `HasToWait` を返す
        if (IsAssetLoading(_task.AssetName)) {
            _task.StartTime = default(DateTime);
            return StartTaskStatus.HasToWait;
        }

        // ロードするアセットが存在しないので `UnknownError` を返す
        if (ResourceLoader.ExistAsset(_task.AssetName)) {
            return StartTaskStatus.UnknownError;
        }

        // 実際にロードリクエストをする
        if (ResourceLoader.LoadRequest(_task.AssetName, OnLoadedAsset)) {
            _task.StartTime = default(DateTime);
            return StartTaskStatus.HasToWait;
        }

        return StartTaskStatus.CanResume;
    }

    /// <summary>
    /// 処理中のタスクを停止して、タスクエージェントをリセットする
    /// </summary>
    public void Reset() {
        _task = default;
    }
#endregion  // ITaskAgent<LoadAssetTask>

    private void OnLoadedAsset(string assetName, object resource) {
        if (assetName != _task.AssetName) {
            return;
        }

        // 自身のリソースが作成されたので、タスクを完了にする
        _task.Done = true;
    }
}
```

* AgentはTaskの情報を元に、実際にリソースを作成する処理を受け持つところです。
* Initializeメソッド
    * Agentの初期化処理を実装してください
    * サンプルでは何もしていません
* Shutdownメソッド
    * Agentの終了処理を実装してください
    * サンプルでは何もしていません
* Updateメソッド
    * Agentのプーリング処理を実装してください
    * サンプルでは何もしていません
* Startメソッド
    * 開始要求のあった、Taskの状態を調べています
    * Taskが既にロード中だったら、処理中とみなし `HasToWait` を返し、TaskPoolのWorkingに移動するよう促します
    * Taskのアセットが存在しない場合 `UnknownError` を返し、TaskPoolにエラーが発生した旨を伝えます
    * Taskのアセットロードリクエストが成功したら `HasToWait` を返し、TaskPoolのWorkingに移動するよう促します
* Resetメソッド
    * Agentの持っている情報をリセットします

### TaskPoolを準備する

```csharp
sealed class AssetManager {
    private TaskPool _taskPool;

    public void Initialize(int agentCapacity) {
        _taskPool = new TaskPool();
        for (var agentIndex = 0; agentIndex < agentCapacity; ++agentIndex) {
            _taskPool.AddAgent(new LoadAssetAgent());
        }
    }

    public void Shutdown() {
        _taskPool.Shutdown();
    }
}
```

* TaskPoolにAgentを必要数登録しています。
* TaskPoolの使用が完了したときの処理を呼び出しています

### TaskPoolを使ってTaskを動かしてみる

```csharp
sealed class AssetManager {
    private TaskPool _taskPool;
    ...
    public void Update(float elapseSeconds, float realElapseSeconds) {
        // タスクの定期処理を実行する
        // ここでタスクが溜まっていたら処理が走り出す
        _taskPool.Update(elapseSeconds, realElapseSeconds);
    }

    public void LoadAsset(string assetName) {
        // 新しいタスクを作成する
        var newTask = LoadAssetTask.Create(assetName);

        // 作成したタスクをTaskPoolに追加する
        _taskPool.AddTask(newTask);
    }
}
```

* TaskPoolの定期処理を行いました
* 新規のTaskを作成・追加し、TaskPoolに登録することで処理が走るようになりました。
