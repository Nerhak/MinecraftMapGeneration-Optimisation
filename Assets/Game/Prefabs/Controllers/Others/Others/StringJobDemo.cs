#if false

using System.Text;
using Unity.Collections;
using Unity.Jobs;

public struct MyJob : IJob
{
	public NativeArray<byte> inStr;
	public NativeArray<byte> outStr;

	public void Execute()
	{
		// Can we use strings inside a job?
		string s = Encoding.ASCII.GetString(inStr.ToArray());
		outStr.CopyFrom(Encoding.ASCII.GetBytes(s));
	}
}

public class JobTestBehaviour : MonoBehaviour
{
	public string testString = "Hello, World!";
	public string resultString = "";

	private void Update()
	{
		MyJob jobData = new MyJob();
		jobData.outStr = new NativeArray<byte>(testString.Length, Allocator.Temp);
		jobData.inStr = new NativeArray<byte>(testString.Length, Allocator.Temp);
		jobData.inStr.CopyFrom(Encoding.ASCII.GetBytes(testString));

		JobHandle handle = jobData.Schedule();
		handle.Complete();
		resultString = Encoding.ASCII.GetString(jobData.outStr.ToArray());

		jobData.inStr.Dispose();
		jobData.outStr.Dispose();
	}
}
#endif