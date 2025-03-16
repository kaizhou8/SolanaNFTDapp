using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NFTClient.Models
{
    /// <summary>
    /// NFT元数据模型
    /// NFT metadata model
    /// </summary>
    public class NFTMetadata
    {
        /// <summary>
        /// NFT名称
        /// NFT name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// NFT符号
        /// NFT symbol
        /// </summary>
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        /// NFT描述
        /// NFT description
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// NFT图片URI
        /// NFT image URI
        /// </summary>
        [JsonPropertyName("image")]
        public string Image { get; set; }

        /// <summary>
        /// 外部URL
        /// External URL
        /// </summary>
        [JsonPropertyName("external_url")]
        public string ExternalUrl { get; set; }

        /// <summary>
        /// 动画URL
        /// Animation URL
        /// </summary>
        [JsonPropertyName("animation_url")]
        public string AnimationUrl { get; set; }

        /// <summary>
        /// NFT属性
        /// NFT attributes
        /// </summary>
        [JsonPropertyName("attributes")]
        public List<NFTAttribute> Attributes { get; set; } = new List<NFTAttribute>();

        /// <summary>
        /// NFT属性
        /// NFT properties
        /// </summary>
        [JsonPropertyName("properties")]
        public NFTProperties Properties { get; set; } = new NFTProperties();

        /// <summary>
        /// 版税百分比 (0-100)
        /// Royalty percentage (0-100)
        /// </summary>
        [JsonPropertyName("seller_fee_basis_points")]
        public int SellerFeeBasisPoints { get; set; }

        /// <summary>
        /// 创建者
        /// Creators
        /// </summary>
        [JsonPropertyName("creators")]
        public List<NFTCreator> Creators { get; set; } = new List<NFTCreator>();
    }

    /// <summary>
    /// NFT属性
    /// NFT attribute
    /// </summary>
    public class NFTAttribute
    {
        /// <summary>
        /// 特征类型
        /// Trait type
        /// </summary>
        [JsonPropertyName("trait_type")]
        public string TraitType { get; set; }

        /// <summary>
        /// 特征值
        /// Trait value
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    /// <summary>
    /// NFT属性
    /// NFT properties
    /// </summary>
    public class NFTProperties
    {
        /// <summary>
        /// 文件
        /// Files
        /// </summary>
        [JsonPropertyName("files")]
        public List<NFTFile> Files { get; set; } = new List<NFTFile>();

        /// <summary>
        /// 类别
        /// Category
        /// </summary>
        [JsonPropertyName("category")]
        public string Category { get; set; }
    }

    /// <summary>
    /// NFT文件
    /// NFT file
    /// </summary>
    public class NFTFile
    {
        /// <summary>
        /// 文件URI
        /// File URI
        /// </summary>
        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        /// <summary>
        /// 文件类型
        /// File type
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// NFT创建者
    /// NFT creator
    /// </summary>
    public class NFTCreator
    {
        /// <summary>
        /// 创建者地址
        /// Creator address
        /// </summary>
        [JsonPropertyName("address")]
        public string Address { get; set; }

        /// <summary>
        /// 分享百分比
        /// Share percentage
        /// </summary>
        [JsonPropertyName("share")]
        public int Share { get; set; }
    }
} 