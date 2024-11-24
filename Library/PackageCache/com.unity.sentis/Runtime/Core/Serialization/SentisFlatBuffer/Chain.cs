// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>
#define ENABLE_SPAN_T
#define UNSAFE_BYTEBUFFER
#define BYTEBUFFER_NO_BOUNDS_CHECK

namespace SentisFlatBuffer
{

using global::System;
using global::System.Collections.Generic;
using global::Unity.Sentis.Google.FlatBuffers;

struct Chain : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
  public static Chain GetRootAsChain(ByteBuffer _bb) { return GetRootAsChain(_bb, new Chain()); }
  public static Chain GetRootAsChain(ByteBuffer _bb, Chain obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public Chain __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int Inputs(int j) { int o = __p.__offset(4); return o != 0 ? __p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int InputsLength { get { int o = __p.__offset(4); return o != 0 ? __p.__vector_len(o) : 0; } }
#if ENABLE_SPAN_T
  public Span<int> GetInputsBytes() { return __p.__vector_as_span<int>(4, 4); }
#else
  public ArraySegment<byte>? GetInputsBytes() { return __p.__vector_as_arraysegment(4); }
#endif
  public int[] GetInputsArray() { return __p.__vector_as_array<int>(4); }
  public int Outputs(int j) { int o = __p.__offset(6); return o != 0 ? __p.bb.GetInt(__p.__vector(o) + j * 4) : (int)0; }
  public int OutputsLength { get { int o = __p.__offset(6); return o != 0 ? __p.__vector_len(o) : 0; } }
#if ENABLE_SPAN_T
  public Span<int> GetOutputsBytes() { return __p.__vector_as_span<int>(6, 4); }
#else
  public ArraySegment<byte>? GetOutputsBytes() { return __p.__vector_as_arraysegment(6); }
#endif
  public int[] GetOutputsArray() { return __p.__vector_as_array<int>(6); }
  public SentisFlatBuffer.Instruction? Instructions(int j) { int o = __p.__offset(8); return o != 0 ? (SentisFlatBuffer.Instruction?)(new SentisFlatBuffer.Instruction()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
  public int InstructionsLength { get { int o = __p.__offset(8); return o != 0 ? __p.__vector_len(o) : 0; } }

  public static Offset<SentisFlatBuffer.Chain> CreateChain(FlatBufferBuilder builder,
      VectorOffset inputsOffset = default(VectorOffset),
      VectorOffset outputsOffset = default(VectorOffset),
      VectorOffset instructionsOffset = default(VectorOffset)) {
    builder.StartTable(3);
    Chain.AddInstructions(builder, instructionsOffset);
    Chain.AddOutputs(builder, outputsOffset);
    Chain.AddInputs(builder, inputsOffset);
    return Chain.EndChain(builder);
  }

  public static void StartChain(FlatBufferBuilder builder) { builder.StartTable(3); }
  public static void AddInputs(FlatBufferBuilder builder, VectorOffset inputsOffset) { builder.AddOffset(0, inputsOffset.Value, 0); }
  public static VectorOffset CreateInputsVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static VectorOffset CreateInputsVectorBlock(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateInputsVectorBlock(FlatBufferBuilder builder, ArraySegment<int> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateInputsVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<int>(dataPtr, sizeInBytes); return builder.EndVector(); }
  public static void StartInputsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddOutputs(FlatBufferBuilder builder, VectorOffset outputsOffset) { builder.AddOffset(1, outputsOffset.Value, 0); }
  public static VectorOffset CreateOutputsVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static VectorOffset CreateOutputsVectorBlock(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateOutputsVectorBlock(FlatBufferBuilder builder, ArraySegment<int> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateOutputsVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<int>(dataPtr, sizeInBytes); return builder.EndVector(); }
  public static void StartOutputsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddInstructions(FlatBufferBuilder builder, VectorOffset instructionsOffset) { builder.AddOffset(2, instructionsOffset.Value, 0); }
  public static VectorOffset CreateInstructionsVector(FlatBufferBuilder builder, Offset<SentisFlatBuffer.Instruction>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static VectorOffset CreateInstructionsVectorBlock(FlatBufferBuilder builder, Offset<SentisFlatBuffer.Instruction>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateInstructionsVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<SentisFlatBuffer.Instruction>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateInstructionsVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<SentisFlatBuffer.Instruction>>(dataPtr, sizeInBytes); return builder.EndVector(); }
  public static void StartInstructionsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<SentisFlatBuffer.Chain> EndChain(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<SentisFlatBuffer.Chain>(o);
  }
}


static class ChainVerify
{
  static public bool Verify(Unity.Sentis.Google.FlatBuffers.Verifier verifier, uint tablePos)
  {
    return verifier.VerifyTableStart(tablePos)
      && verifier.VerifyVectorOfData(tablePos, 4 /*Inputs*/, 4 /*int*/, false)
      && verifier.VerifyVectorOfData(tablePos, 6 /*Outputs*/, 4 /*int*/, false)
      && verifier.VerifyVectorOfTables(tablePos, 8 /*Instructions*/, SentisFlatBuffer.InstructionVerify.Verify, false)
      && verifier.VerifyTableEnd(tablePos);
  }
}

}