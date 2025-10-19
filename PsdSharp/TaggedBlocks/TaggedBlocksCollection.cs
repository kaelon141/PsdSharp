namespace PsdSharp.TaggedBlocks;

public class TaggedBlocksCollection(List<TaggedBlock> blocks)
{
    public IReadOnlyCollection<TaggedBlock> Blocks => blocks;

    public T? TryGet<T>() where T : TaggedBlock
    {
        #if NET6_0_OR_GREATER
        var key = TaggedBlocksRegistry.KeyByLayer.GetValueOrDefault(typeof(T));
        #else
        var key = TaggedBlocksRegistry.KeyByLayer.TryGetValue(typeof(T), out var taggedBlock) ? taggedBlock : null;
        #endif
        
        if (key is null)
            return null;

        return TryGet(key) as T;
    }
    
    public TaggedBlock? TryGet(AdditionalLayerInfoKey key)
        => TryGet(key.Key);

    public TaggedBlock? TryGet(string key) => blocks.Find(block => block.Key.Key == key);
}