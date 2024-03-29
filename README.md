# RTask

## A simple efficient mainThread allocation free async/await integration for Unity.

1. Custom AsyncMethodBuilder to achieve zero allocation.

2. PlayerLoop based task(RTask.Delay, RTask.DelayFrame, RTask.DelayAction etc..)

## Simple Example

```csharp
    
private async void TestRTaskInt()
{
    var res = await TestRTaskInt32();
    Debug.Log(res); // log: 111
}

private RTask<int> TestRTaskInt32()
{
    RTask<int> rTask = RTask<int>.Create();
    rTask.SetResult(111);
    return rTask;
}
    
private async void TestRTask()
{
    //milliseconds, use unscaledDealtTime
    await RTask.Delay(1000, true);

    await RTask.DelayFrame(10);

    await RTask.NextFrame();

    await RTask.EndOfFrame();

    await RTask.DelayFrame(0);
}

private void TestAction()
{
    RTask.DelayFrameAction(111, () =>
    {
        Debug.Log("DelayFrameAction");
    });
    //milliseconds
    RTask.DelayAction(2000, () =>
    {
        Debug.Log("DelayAction");
    });
}
````
