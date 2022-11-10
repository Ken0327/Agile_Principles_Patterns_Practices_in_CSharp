測試
======

## 1. 測試驅動開發
- 開發原則
    - 在撰寫一個單元測試前，不撰寫任何的程式碼
    - 只撰寫剛好無法通過的單元測試，不能編譯也算無法通過
    - 只撰寫剛好能通過測試失敗的產品程式碼

- 先撰寫測試，有助於設計出便於呼叫 (Conveniently callable) 的軟體
- 迫使自己將產品程式設計成可測試的 (testable) ，有助於去除軟體間的耦合
- 測試可以作為一個重要的說明文件，亦可作為程式的範例來使用。

## 2. 先寫測試的例子
```csharp
[Test]
public void TestMove()
{
	WumpusGame g = new WumpusGame();
	g.Connect(4,5,"E");
	g.GetPlayerRoom(4);
	g.East();
	Assert.AreEqual(5,g.GetPlayerRoom());
}
````

- 先在測試中陳述意圖，並使意圖盡可能的簡單、易讀。基於測試驅動開發的原則，將有助於對於產品程式的設計
    - 例如在此例中就可以簡化對於Room類別的建構。
- 先寫測試的行為就是在各種設計決策中，進行辨別的行為。


## 2. 測試促使模組之間隔離

![Test_1]](./test_1.png "Test.md")
>耦合在一起的薪水支付應用模型

-  可能問題
    - 對資料庫的溝通? 需要撰寫完整資料庫功能?
    - 如何驗證支票的正確性?

- Mock Object 的設計

![Test_1]](./test_2.png "Test.md")
> 使用Mock Object 來測試，解除了耦合的薪水支付模型

- 透過Mock Object 作為介面的替身使用
- 透過Mock Object 去驗證/測試介面的工作

```csharp
[Test]
public void TessPayroll()
{
	MockEmployeeDataBase db = new MockEmployeeDataBase();
    MockCheckWriter w = new MockCheckWriter();
    Payroll p = new Payroll(db,w);
    p.PayEmployees();
    Assert.IsTrue(w.ChecksWereWrittenCorrectly());
    Assert.IsTrue(db.PaymentWerePostedCorrectly());
}
````

- 意外獲得的解耦合
    - 為了設計可以被測試的程式，迫使對模組進行了隔離，增加了應用的擴展性。
    - 在撰寫程式碼之前，寫測試改善了設計。


## 3. 驗收測試

- 驗收當系統是作為一個整體時的工作正確性

- 單元測試 : 驗證系統中單個機制的白盒測試
- 驗收測試 : 驗證系統滿足客戶需求的黑盒測試

- 驗收測試是真正的需求文件
    - 有關乎系統的可編譯性，可執行性的說明文件。

-  先寫驗收測試的重要性
    - 為了使系統具備可測試性，需要在較高的系統架構層面對系統進行解耦。
    - 自動化驗收系統可做為對系統進行解耦的動力

- 意外獲得的架構
    - 在此例中為了把薪水支付系統設計成一個可重用的架構，對應使用的方法就是API函式。
    - 透過API 完成功能，用以解耦

## 4. 總結
- 測試套件運行起來越簡單，就越會被頻繁的執行，測試的越多，就會越快的發現與測試背離的事。
- 單元測試和驗收測試都可作為一種說明文件的形式存在。
- 傳寫測試所使用的語言是明確的：
    - 單元測試使用程式語言撰寫，程式設計師可以閱讀
    - 驗收測試使用間單的表格語言撰寫，客戶能夠閱讀驗收測試

- 測試對架構和設計有重大的影響，為了使模組或程式具備可測試性，必須要對其進行解耦，優化產品程式的架構設計