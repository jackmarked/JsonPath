namespace JetDevel.JsonPath;
public abstract class UnicodeCharacterReader
{
    public abstract bool TryReadNext(out UnicodeCharacter character);
}