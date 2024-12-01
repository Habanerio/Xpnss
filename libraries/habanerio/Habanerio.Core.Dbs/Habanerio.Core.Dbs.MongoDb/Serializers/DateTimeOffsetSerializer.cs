// using MongoDB.Bson;
// using MongoDB.Bson.IO;
// using MongoDB.Bson.Serialization;
// using MongoDB.Bson.Serialization.Serializers;
//
// namespace Habanerio.Core.Dbs.MongoDb.Serializers;
//
// public class DateTimeSerializer : SerializerBase<DateTime>
// {
//     public static readonly DateTimeSerializer Instance = new();
//
//     private static class Fields
//     {
//         public const string DateTime = "DateTime";
//         public const string LocalDateTime = "LocalDateTime";
//         public const string Ticks = "Ticks";
//         public const string Offset = "Offset";
//     }
//
//     public override void Serialize(
//         BsonSerializationContext context,
//         BsonSerializationArgs args,
//         DateTime value)
//     {
//         context.Writer.WriteStartDocument();
//
//         context.Writer.WriteName(Fields.DateTime);
//         context.Writer.WriteDateTime(
//             BsonUtils.ToMillisecondsSinceEpoch(value.ToUniversalTime()));
//
//         context.Writer.WriteName(Fields.LocalDateTime);
//         context.Writer.WriteDateTime(
//             BsonUtils.ToMillisecondsSinceEpoch(value.ToUniversalTime().Add(value.Offset)));
//
//         context.Writer.WriteName(Fields.Offset);
//         context.Writer.WriteInt32(value.Offset.Hours * 60 + value.Offset.Minutes);
//
//         context.Writer.WriteName(Fields.Ticks);
//         context.Writer.WriteInt64(value.Ticks);
//
//         context.Writer.WriteEndDocument();
//     }
//
//     public override DateTime Deserialize(
//         BsonDeserializationContext context,
//         BsonDeserializationArgs args)
//     {
//         context.Reader.ReadStartDocument();
//
//         context.Reader.ReadName();
//         context.Reader.SkipValue();
//
//         context.Reader.ReadName();
//         context.Reader.SkipValue();
//
//         context.Reader.ReadName();
//         var offset = context.Reader.ReadInt32();
//
//         context.Reader.ReadName();
//         var ticks = context.Reader.ReadInt64();
//
//         context.Reader.ReadEndDocument();
//
//         return new DateTime(ticks, TimeSpan.FromMinutes(offset));
//     }
// }