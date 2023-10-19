# JSFW.TablePrint
Table 데이타를 Text로 출력.

테이블 형태의 자료형을 Text표로 그리는 프로그램

- 출력결과
![image](https://user-images.githubusercontent.com/116536524/197675480-1e9859fc-f457-49b1-afd2-3d588593665b.png)



- 출력결과
<pre><code>
: 헤더가 1개 행일때...
Table tb = new Table("T1"); 
tb.AddRowData("A", "A", "A", "F", "H");
tb.AddRowData("B", "B", "C", "F", "G");
tb.AddRowData("D", "E", "C", "F", "G");
tb.AddRowData("Z", "Y", "X", "W", "P"); 
tb.HeaderCount = 1;
출력 1개 행일때..
</code></pre>
![image](https://user-images.githubusercontent.com/116536524/197675480-1e9859fc-f457-49b1-afd2-3d588593665b.png)




- 출력 결과
<pre><code>
: 헤더가 4개 행일때...
Table tb = new Table("T1"); 
tb.AddRowData("A", "A", "A", "F", "H");
tb.AddRowData("B", "B", "C", "F", "G");
tb.AddRowData("D", "E", "C", "F", "G");
tb.AddRowData("Z", "Y", "X", "W", "P"); 
tb.HeaderCount = 4;
</code></pre>
![image](https://github.com/aseuka/JSFW.TablePrint/assets/116536524/cc13d19a-336c-4676-a887-a43affdb394a)
