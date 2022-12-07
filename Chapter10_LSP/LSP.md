Liskov 替換原則
======
> 子型態(Subtype)必須能夠替換掉它們的基底型態(base type)
> 當一個函式f()，他的參數為指向某個基底類別為B的參考，而假設B有衍生類別D，如果把D的物件作為B型態傳遞給f()，若會導致f()錯誤，則代表此設計違反了LSP。

## 1. 違反LSP的情境

* 違反了OCP，因為DrawShape()這個動作必須要知道Shape的所有類別，才可以透過if/else...的方式判斷，且每次建立新類別時，都必須要進行重大修正
* 違反了LSP，因為Square / Circle類別無法替換Shape類別
* 通常對於LSP的違反也潛在的違反了OCP

```csharp
struct Point {double x,y}

public enum ShapeType {square,circle};

public class Shape
{
    private ShapeType type;

    public Shape (ShapeType t){type= t; }
    public static void DrawShape(Shape s)
    {
        if(s.type == ShapeType.square)
            (s as Square).Draw();
        else if(s.type == ShapeType.circle)
            (s as Circle).Draw();    
    }
    public class Circle : Shape
    {
        private Point center;
        private double redius;

        public Circle() : base(ShapeType.circle) {}
        public void Draw(){}
    }

    public class Square : Shape
    {
        private Point topLeft;
        private double side;

        public Square() : base(ShapeType.square) {}
        public void Draw(){}
    }
}
```

* 使用Rectangle物件的函式，無法正確的操作Square物件。對於這些函式來說，Square不能替換Rectangle，故可知這樣是違反LSP的。

``` csharp
public class Rectangle
{
    private Point topLeft;
    private double width;
    private double height;
    
    public virtual double Width
    {
        get { return width; }
        set{ width = value; }
    }

    public virtual double Height
    {
        get { return height; }
        set{ height = value; }
    }
}

public class Square : Rectange
{
    public override double Width
    {
        set
        {
            base.Width = value;
            base.Height = value;
        }
    }

    public override double Height
    {
        set
        {
            base.Width = value;
            base.Height = value;
        }
    }
}
```

* 有效性非本質屬性：
	* 一個模型，如果獨立來看待，並不具有真正意義上的有效性。
	* 模型的有效性只能透過他的客戶程式來表現。
	* 在判斷某個特定設計是否恰當時，不能完全獨立看待這個解決方案。必須根據該設計的使用者所做出的合理假設來審查。
	* 不需要預測所有的假設，只需要預測那些最明顯違反LSP的情況。
	

* IS-A是關於行為的：
	* 從行為方式的角度來看，Square不是Rectangle，物件的行為才是軟體真正關注的問題。
	* OOD中IS-A的關係是從行為來判斷的，而行為是可以合理假設的，也是客戶程式所依賴的
	
* 基於契約設計：
	* 基於契約的設計(Design by contract):使合理的假設明確化。必須要滿足前置/後置條件
		* 宣告前置條件 (precondition): 要使一個方法可以執行，前置條件必為真。
		* 宣告後置條件 (postcondition):方法執行完畢後，該方法要保證後置條件為真。
	* 在重新宣告衍生類別的副程式時，只能使用相等或更鬆的前置條件來替換原始的前置條件。只能使用相等或更強的後置條件來替換原始的後置條件。
	
```csharp
// Rectangle.Width postcondition
assert((width == w) && (height == old.value));
```


* 在單元測試中指定契約：
	* 可以透過撰寫單元測試的方式來指定契約。
	* 單元測試透過徹底的側是一個類別的行為，使該類別的行為更加清晰。




## 2. 用提取共同部分的方法代替繼承
* 只有在極少數的情況下，接受一個多型行為中的微妙錯誤 才會比 試著修改設計使之完全符合 LSP 更有利。
* 修改前：

```csharp
// Line.cs
public class Line
{
    private Point p1;

    private Point p2;

    public Line(Point p1, Point p2) {this.p1 = p1; this.p2 = p2; }
    
    public Point P1 {get {return p1;}}
    public Point P2 {get {return p2;}}
    public DOUBLE sLOPE {get {}}
    public double Yintercept {get{}}
    public virtual bool IsOn(Point p) {} 
}


// LineSegment.cs

public class LineSegment : Line
{
    public LineSegment(Point p1, Point p2) :base(p1,p2)  {}

    public double Length() {get{}}
    public override bool IsOn(Point p) {}
}
```
* 修改後：
	* 將類別Line和類別LineSegment的共同部分抽取出來做為一個抽象基底類別
	* 如果每一組類別都支援一個共同的職責，那麼它們應該從一個共同的超類別繼承該職責
	
```csharp
// LinearObject.cs
public abstract class LinearObject
{
    private Point p1;
    private Point p2;
    
    public LinearObject(Point p1,Point p2)
    {
        this.p1 = p1;
        this.p2 = p2;
    }

    public Point P1 {get { return p1;}}
    public Point P2 {get { return p2;}}

    public double Slop3 { get {}}
    public double Yintercept {get {}}
    public virtual bool IsOn(Poiont p){}
}

// Line.cs
public class Line :LinearObject
{
    public Line(Point p1,Point p2) : base(p1,p2) {}
    public override bool IsOn(Point p) {}
}

// LineSegment.cs
public class LineSegment :LinearObject
{
    public LineSegment(Point p1,Point p2) : base(p1,p2) {}

    public double GetLength() {}
    
    public override bool IsOn(Point p) {}
}
```

## 3. 啟發式規則和習慣用法
* 以某種方式從基底類別中去除功能的衍生類別
* 完成的功能少於基底類別的衍生類別，通常是不能替換基底類別的，因為這樣就違反了LSP
* 在衍生類別中存在退化函式並不總是代表違反了LSP，但是當出現這種情況時，還是值得關注的。

``` csharp
// 衍生類別中的退化函式
public class base
{
    public virtual void f() 
    {
        // Some thing need to do 
    }
}

public class Derived : base
{
    public override void f() {}
}
```


	
## 4. 總結
* OCP是OOP中許多主張的核心，如果能有效地應用這個原則，應用程式就會具有更強的可維護性、在使用性及強固性。
* 透過 LSP實現子型態的可替換性，才使得使用基底類別型態表示的模組，在無需修改的情況下就得以擴展。
* IS-A的涵義比較廣泛，較無法作為子型態的定義。
* 子型態的正確定義應該是可替換的，這裡的可替換性可以透過顯示或隱式的契約來定義。