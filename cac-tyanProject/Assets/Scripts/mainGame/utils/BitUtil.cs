
public class BitUtil {
	
	public static bool Exist(int a,int b){
		return ((a & b) != 0);
	}
	public static void Add(ref int a,int b){
		a = a | b;
	}
	public static void Loss(ref int a,int b){
		a = a & ~b;
	}
	public static void Xor(ref int a,int b){
		a = a ^ b;
	}

}
