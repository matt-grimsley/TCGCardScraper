namespace TCGCardScraper.Scryfall.Models;
public class ScryfallCard
{
    static public string Obj => "card";
    public string? Id { get; set; }
    public string? OracleId { get; set; }
    public string? MultiverseId { get; set; }
    public string? MtgoId { get; set; }
    public string? ArenaId { get; set; }
    public string? TcgplayerId { get; set; }
    public string? CardmarketId { get; set; }
    public string? Name { get; set; }
    public string? Lang { get; set; }
    public string? ReleasedAt { get; set; }
    public string? Uri { get; set; }
    public string? ScryfallUri { get; set; }
    public ImageUris? ImageUris { get; set; }
    public string? ManaCost { get; set; }
    public float? Cmc { get; set; }
    public string? TypeLine { get; set; }
    public string? OracleText { get; set; }
    public List<string>? Keywords { get; set; }
    public List<string>? ColorIdentity { get; set; }
    public List<string>? ColorIndicator { get; set; }
    public List<string>? Colors { get; set; }
    public string? Power { get; set; }
    public string? Toughness { get; set; }
    public string? Loyalty { get; set; }
    public string? FlavorText { get; set; }
    public string? IllustrationId { get; set; }
    public string? Artist { get; set; }
    public string? BorderColor { get; set; }
    public string? Frame { get; set; }
    public bool? FullArt { get; set; }
    public bool? Textless { get; set; }
    public bool? Booster { get; set; }
    public string? Watermark { get; set; }
    public bool? StorySpotlight { get; set; }
    public Dictionary<string, string>? RelatedUris { get; set; }
    public Dictionary<string, string>? PurchaseUris { get; set; }
    public string? Set { get; set; }
    public string? SetName { get; set; }
    public string? SetType { get; set; }
    public string? SetUri { get; set; }
    public string? SetSearchUri { get; set; }
    public string? ScryfallSetUri { get; set; }
    public string? RulingsUri { get; set; }
    public string? PrintsSearchUri { get; set; }
    public string? CollectorNumber { get; set; }
    public bool? Digital { get; set; }
    public bool? Nonfoil { get; set; }
    public bool? Foil { get; set; }
    public bool? Oversized { get; set; }
    public bool? Promo { get; set; }
    public bool? Reprint { get; set; }
    public bool? Variation { get; set; }
    public string? CardBackId { get; set; }
    public List<RelatedCard>? RelatedCards { get; set; }
    public List<CardFace>? CardFaces { get; set; }
}

public class ImageUris
{
    public string? Small { get; set; }
    public string? Normal { get; set; }
    public string? Large { get; set; }
    public string? Png { get; set; }
    public string? ArtCrop { get; set; }
    public string? BorderCrop { get; set; }
}

public class RelatedCard
{
    static public string Obj => "card";
    public string? Name { get; set; }
    public string? Set { get; set; }
    public string? CollectorNumber { get; set; }
}

public class CardFace
{
    static public string Obj => "card";
    public string? Name { get; set; }
    public string? ManaCost { get; set; }
    public string? TypeLine { get; set; }
    public string? OracleText { get; set; }
    public string? Power { get; set; }
    public string? Toughness { get; set; }
    public string? FlavorText { get; set; }
    public ImageUris? ImageUris { get; set; }
}
