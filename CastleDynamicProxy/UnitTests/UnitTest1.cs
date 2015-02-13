using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Castle.Components.DictionaryAdapter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace UnitTests
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			var dict = new System.Collections.Generic.Dictionary<string, object>();
			dict["Argument1"] = "A1";
			dict["Argument2"] = "A2";

			IMyInt wrapper = new DictionaryAdapterFactory().GetAdapter<IMyInt>(dict);
			Assert.AreEqual("A1", wrapper.Argument1);
			Assert.AreEqual("A2", wrapper.Argument2);

			dict["Argument3"] = 3;
			Assert.AreEqual("A3", wrapper.Argument3);
		}

		[TestMethod]
		public void MyTestMethod()
		{
			var o = new MyClass { Argument1 = "Homer", Argument2 = "Jay", Argument3 = "Simpsons" };
			var json = JsonConvert.SerializeObject(o);
			Console.WriteLine(json);
		}

		[TestMethod]
		public void MyTestMethod2()
		{
			const string json = "{\"Argument1\":\"Homer\",\"Argument2\":\"Jay\",\"Argument3\":\"Simpsons\"}";
			JObject j = JObject.Parse(json);
			var dict = j.Properties().ToDictionary(jp => jp.Name, jp => jp.Value.ToString());
			IMyInt wrapper = new DictionaryAdapterFactory().GetAdapter<IMyInt>(dict);
			Assert.AreEqual("Homer", wrapper.Argument1);
			Assert.AreEqual("Jay", wrapper.Argument2);
			Assert.AreEqual("Simpsons", wrapper.Argument3);
		}
		public interface IMyInt
		{
			string Argument1 { get; set; }
			string Argument2 { get; set; }
			string Argument3 { get; set; }
		}

		public class MyClass : IMyInt
		{

			public string Argument1
			{
				get;
				set;
			}

			public string Argument2
			{
				get;
				set;
			}

			public string Argument3
			{
				get;
				set;
			}
		}
	}
}
