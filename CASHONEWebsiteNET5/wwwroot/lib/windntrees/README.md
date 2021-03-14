<h1>WindnTrees</h1>
<p>WindnTrees CRUDS is a front-end http request, response and related content processing javascript library that integrates with Observers to synchronize data between source and view objects that follows the create, read, update and delete (CRUD) data extraction, editing, deletion and saving design pattern.</p>
<p>CRUDS utilize CRUDController to extract data from a data source or CRUDSource and processes response with CRUDProcessor to produce typed objects. Observers utilize typed objects and present them in their appropriate views.</p>
<p>This simplifies the process of data extraction, processing and presentation.</p>
<p>Learn CRUD, CRUD2CRUD (CRUDS) programming in WindnTrees JavaScript, ASP .NET and ASP .NET Core. Develop applications with minimal effort using client and server side CRUD controllers and views.</p>
<p>Benefits:</p>
<ol>
  <li>MVVM</li>
  <li>CRUD2CRUD Controllers (Generalized Communication)</li>
  <li>CRUD Repositories for persistence</li>
  <li>Neat Views and Scripting</li>
  <li>Professional Practice, and</li>
  <li>Simple Codebase</li>
</ol>
<p>Get started with WindnTrees application development by learning following:</p>
<p><a href="http://www.invincibletec.com/tutorial/detail/getting-started-with-windntrees" title="getting-started-with-windntrees">Tutorial</a></p>

<h2>CRUD2CRUD (CRUD=CRUD) Architecture</h2>
<p>WindnTrees implements CRUD 2 CRUD (CRUDS parallel) application architecture where server side publish services in terms of CRUD controllers and the client side consumes those services using CRUD controllers.</p>

<pre>
[Server Side]
CRUDSource(s) --- CRUDController(s) -- CRUDProcessor(s) --- CRUDService(s)

              ||| data / objects / entities / concepts ||| 

[Client Side]
CRUDSource(s) --- CRUDController(s) -- CRUDProcessor(s) --- CRUDConsumer(s)
</pre>

<h2>Dependencies</h2>
        <p>
            <a href="#">CRUDS</a>
            is developed to be open and independent library, its functionality can be extended or customized for content extraction, processing and presentation.
        </p>
        <p>The library depends on <a href="https://jquery.com/">jQuery</a> for making AJAX calls to extract content and utilizes <a href="http://knockoutjs.com/">KnockoutJs</a> for presenting processed content.</p>
<p>CRUDS expects server side content response presented in following JSON format :</p>
<pre>
{
      content: {},
      errors: [] 
}
</pre>
<pre>
{
      contents: [],
      errors: []
}
</pre>
        <p>
            Where <em>errors</em> field is a list or array of error objects composed of "Field Name" and "Error Message" while <em>contents</em> is a list or array of returned response objects.
        </p>


<h2>Download</h2>
<h3>.NET Framework Standard</h3>
<p>WindnTrees abstraction library is available as a nuget package download that can be installed using following command.</p>
<p>install-package WindnTrees.Abstraction</p>

<h3>.NET Framework Core</h3>
<p>WindnTrees .NET Core nuget package is available for download to implement ASP .NET Core based applications.</p>

<p>install-package WindnTrees.Core</p>

<p>Note: nuget package manager does not install windntrees.min.js in correct project scripts library directory, make sure you recover script files from C:\Users\&lt;&lt;Username&gt;&gt;\.nuget\packages\. Or Download from following url <a href="https://github.com/shamszia/windntrees">here</a>.</p>

<h3>Maven / Thymeleaf / Java</h3>
<p>You may resolve windntrees application references using following:</p>

<pre>
    <dependency>
        <groupId>com.invincibletec.windntrees</groupId>
        <artifactId>windntrees</artifactId>
        <version>version-0.0.1</version>
    </dependency>
</pre>

<p>Note: windntrees-1.2.3.js updates are not applicable to com.invincibletec.windntrees as of now, repository will be updated soon.</p>
