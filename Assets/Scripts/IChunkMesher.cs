namespace Miren
{
	public interface IChunkMesher
	{
		void GenerateMesh(World world, Chunk chunk, MeshData meshData);
	}
}
