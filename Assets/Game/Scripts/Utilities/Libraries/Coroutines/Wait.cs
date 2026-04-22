namespace Library.Coroutines
{
	using UnityEngine;
	
	public static class Wait
	{
		public static WaitForEndOfFrame		ForEndOfFrame	{ get; private set; } = new WaitForEndOfFrame();
		public static WaitForFixedUpdate	ForFixedUpdate	{ get; private set; } = new WaitForFixedUpdate();
		public static WaitForSeconds		ForOneSecond	{ get; private set; } = new WaitForSeconds(1);
		public static WaitForSeconds		ForFourSeconds	{ get; private set; } = new WaitForSeconds(4);
	}
}