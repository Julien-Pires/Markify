namespace Markify.CodeAnalyzer.Roslyn.VisualBasic

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.VisualBasic
open Microsoft.CodeAnalysis.VisualBasic.Syntax

[<AbstractClass>]
type TypesCollector<'a>() =
    inherit VisualBasicSyntaxWalker()

    let mutable result : 'a list = []

    member __.Visit(node) = 
        base.Visit(node)
        result

    member __.add = function
        | Some x -> x::result
        | None -> result

    abstract member VisitClassBlock : ClassBlockSyntax -> 'a option

    abstract member VisitStructureBlock : StructureBlockSyntax -> 'a option

    abstract member VisitInterfaceBlock : InterfaceBlockSyntax -> 'a option

    abstract member VisitDelegateStatement : DelegateStatementSyntax -> 'a option

    abstract member VisitEnumBlock : EnumBlockSyntax -> 'a option

    override this.VisitClassBlock node =
        result <- this.add <| this.VisitClassBlock(node)
        base.VisitClassBlock(node)

    override this.VisitStructureBlock node =
        result <- this.add <| this.VisitStructureBlock(node)
        base.VisitStructureBlock(node)

    override this.VisitInterfaceBlock node =
        result <- this.add <| this.VisitInterfaceBlock(node)
        base.VisitInterfaceBlock(node)

    override this.VisitDelegateStatement node =
        result <- this.add <| this.VisitDelegateStatement(node)
        base.VisitDelegateStatement(node)

    override this.VisitEnumBlock node =
        result <- this.add <| this.VisitEnumBlock(node)
        base.VisitEnumBlock(node)

[<AbstractClass>]
type MembersCollector<'a>() =
    inherit VisualBasicSyntaxVisitor<'a option>()
    
    member this.Visit(node : TypeBlockSyntax) =
        node.Members
        |> Seq.choose this.Visit
        |> Seq.toList

type TypesCollector() =
    inherit TypesCollector<SyntaxNode>()
    override __.VisitClassBlock (node : ClassBlockSyntax) = Some (node :> SyntaxNode)
    override __.VisitStructureBlock (node : StructureBlockSyntax) = Some (node :> SyntaxNode)
    override __.VisitInterfaceBlock (node : InterfaceBlockSyntax) = Some (node :> SyntaxNode)
    override __.VisitEnumBlock (node : EnumBlockSyntax) = Some (node :> SyntaxNode)
    override __.VisitDelegateStatement (node : DelegateStatementSyntax) = Some (node :> SyntaxNode)

type FieldSyntaxCollector() =
    inherit MembersCollector<FieldDeclarationSyntax>()
    override __.VisitFieldDeclaration (node) = Some node

type EventSyntaxCollector() =
    inherit MembersCollector<EventStatementSyntax>()
    override __.VisitEventBlock (node) = Some (node.EventStatement)
    override __.VisitEventStatement(node) = Some node

type PropertyBlockSyntaxCollector() =
    inherit MembersCollector<PropertyBlockSyntax>()
    override __.VisitPropertyBlock (node) = Some node

type PropertyStatementSyntaxCollector() =
    inherit MembersCollector<PropertyStatementSyntax>()
    override __.VisitPropertyStatement (node) = Some node

type MethodSyntaxCollector() =
    inherit MembersCollector<MethodStatementSyntax>()
    override __.VisitMethodBlock(node) = Some (node.BlockStatement :?> MethodStatementSyntax)
    override __.VisitMethodStatement(node) = Some node