using Godot;
using System.Collections.Generic;

public partial class InfiniteRoad : Node2D
{

	[Export]
    public int chunkSize = 1008;
	[Export]
	public Node2D chunkProto;
    private Dictionary<int, Node> loadedChunks = new Dictionary<int, Node>();

    public override void _Ready()
    {
        base._Ready();

		CreateTween().SetLoops().TweenCallback(Callable.From(() => {
			Camera2D cam = GetViewport().GetCamera2D();
			if (cam == null) return;
			float camHalfW = GetViewport().GetVisibleRect().Size.X / cam.Zoom.X / 2.0f;
			float left = cam.GetScreenCenterPosition().X - camHalfW;
			float right = cam.GetScreenCenterPosition().X + camHalfW;
			UpdateChunks((int) left, (int) right);
			// GD.Print(left, " ", right);
		})).SetDelay(0.2f);
    }

    public Node LoadChunk(int chunkId)
    {
		GD.Print("Load ", chunkId);
        Node2D newChunk = (Node2D) chunkProto.Duplicate();
		chunkProto.GetParent().AddChild(newChunk);
		newChunk.Position = new Vector2(chunkId * chunkSize, 0);
		newChunk.Show();
		return newChunk;
    }

    public void UnloadChunk(int chunkId)
    {
		GD.Print("Unload ", chunkId);
        Node2D unwantedChunk = (Node2D) loadedChunks[chunkId];
		unwantedChunk.QueueFree();
    }

    public void UpdateChunks(int left, int right)
    {
        int leftId = left / chunkSize;
        int rightId = right / chunkSize;

        // Load new chunks
        for (int i = leftId; i <= rightId; i++)
        {
            if (!loadedChunks.ContainsKey(i))
            {
                LoadChunk(i);
                loadedChunks.Add(i, LoadChunk(i));
            }
        }

        // Unload unwanted chunks
        List<int> ids = new List<int>(loadedChunks.Keys);
        for (int i = 0; i < ids.Count; i++)
        {
            if (ids[i] < leftId || ids[i] > rightId)
            {
                UnloadChunk(ids[i]);
                loadedChunks.Remove(ids[i]);
            }
        }
    }
}
