using System;

public class Test {
	static void Main() {
		NETLibrary.Communication co = new NETLibrary.Communication(1);
		co.SetListenData("8.8.8.8","6300");
	}
}