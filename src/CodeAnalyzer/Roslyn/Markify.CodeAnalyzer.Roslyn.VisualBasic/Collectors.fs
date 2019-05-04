namespace Markify.CodeAnalyzer.Roslyn.VisualBasic

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.VisualBasic
open Microsoft.CodeAnalysis.VisualBasic.Syntax

[<AbstractClass>]
type VisualBasicSyntaxVisitor<'a>(value : 'a) =
    inherit VisualBasicSyntaxVisitor()

    let mutable result : 'a = value

    member __.Visit(node) =
        base.Visit(node)
        result

    abstract member VisitFieldDeclaration : FieldDeclarationSyntax * 'a -> 'a
    default __.VisitFieldDeclaration (_, result) = result

    abstract member VisitEventBlock : EventBlockSyntax * 'a -> 'a
    default __.VisitEventBlock (_, result) = result

    abstract member VisitEventStatement : EventStatementSyntax * 'a -> 'a
    default __.VisitEventStatement (_, result) = result

    abstract member VisitPropertyBlock : PropertyBlockSyntax * 'a -> 'a
    default __.VisitPropertyBlock (_, result) = result

    abstract member VisitPropertyStatement : PropertyStatementSyntax * 'a -> 'a
    default __.VisitPropertyStatement (_, result) = result

    abstract member VisitMethodBlock : MethodBlockSyntax * 'a -> 'a
    default __.VisitMethodBlock (_, result) = result

    abstract member VisitMethodStatement : MethodStatementSyntax * 'a -> 'a
    default __.VisitMethodStatement (_, result) = result

    override this.VisitFieldDeclaration node =
        result <- this.VisitFieldDeclaration(node, result)
        base.VisitFieldDeclaration(node)

    override this.VisitEventBlock node =
        result <- this.VisitEventBlock(node, result)
        base.VisitEventBlock(node)

    override this.VisitEventStatement node =
        result <- this.VisitEventStatement(node, result)
        base.VisitEventStatement(node)

    override this.VisitPropertyBlock node =
        result <- this.VisitPropertyBlock(node, result)
        base.VisitPropertyBlock(node)

    override this.VisitPropertyStatement node =
        result <- this.VisitPropertyStatement(node, result)
        base.VisitPropertyStatement(node)

    override this.VisitMethodBlock node =
        result <- this.VisitMethodBlock(node, result)
        base.VisitMethodBlock(node)

    override this.VisitMethodStatement node =
        result <- this.VisitMethodStatement(node, result)
        base.VisitMethodStatement(node)

[<AbstractClass>]
type VisualBasicSyntaxWalker<'a>(value : 'a) =
    inherit VisualBasicSyntaxWalker()

    let mutable result : 'a = value

    member __.Visit(node) =
        base.Visit(node)
        result

    abstract member VisitClassBlock : ClassBlockSyntax * 'a -> 'a
    default __.VisitClassBlock(_, result) = result

    abstract member VisitStructureBlock : StructureBlockSyntax * 'a -> 'a
    default __.VisitStructureBlock(_, result) = result

    abstract member VisitInterfaceBlock : InterfaceBlockSyntax * 'a -> 'a
    default __.VisitInterfaceBlock(_, result) = result

    abstract member VisitDelegateStatement : DelegateStatementSyntax * 'a -> 'a
    default __.VisitDelegateStatement(_, result) = result

    abstract member VisitEnumBlock : EnumBlockSyntax * 'a -> 'a
    default __.VisitEnumBlock(_, result) = result

    abstract member VisitNamespaceBlock : NamespaceBlockSyntax * 'a -> 'a
    default __.VisitNamespaceBlock(_, result) = result

    abstract member VisitIdentifierName : IdentifierNameSyntax * 'a -> 'a
    default __.VisitIdentifierName(_, result) = result

    override this.VisitClassBlock node =
        result <- this.VisitClassBlock(node, result)
        base.VisitClassBlock(node)

    override this.VisitStructureBlock node =
        result <- this.VisitStructureBlock(node, result)
        base.VisitStructureBlock(node)

    override this.VisitInterfaceBlock node =
        result <- this.VisitInterfaceBlock(node, result)
        base.VisitInterfaceBlock(node)

    override this.VisitDelegateStatement node =
        result <- this.VisitDelegateStatement(node, result)
        base.VisitDelegateStatement(node)

    override this.VisitEnumBlock node =
        result <- this.VisitEnumBlock(node, result)
        base.VisitEnumBlock(node)

    override this.VisitNamespaceBlock node =
        result <- this.VisitNamespaceBlock(node, result)
        base.VisitNamespaceBlock(node)

    override this.VisitIdentifierName node =
        result <- this.VisitIdentifierName(node, result)
        base.VisitIdentifierName(node)

type NamespaceSyntaxCollector() =
    inherit VisualBasicSyntaxWalker<NamespaceBlockSyntax list>([])
    override __.VisitNamespaceBlock (node, result) = node::result

type TypeCollector() =
    inherit VisualBasicSyntaxWalker<SyntaxNode list>([])
    override __.VisitClassBlock (node, result) = (node :> SyntaxNode)::result
    override __.VisitStructureBlock (node, result) = (node :> SyntaxNode)::result
    override __.VisitInterfaceBlock (node, result) = (node :> SyntaxNode)::result
    override __.VisitEnumBlock (node, result) = (node :> SyntaxNode)::result
    override __.VisitDelegateStatement (node, result) = (node :> SyntaxNode)::result

type FieldSyntaxCollector() =
    inherit VisualBasicSyntaxVisitor<FieldDeclarationSyntax list>([])
    override __.VisitFieldDeclaration (node, result) = node::result

type EventSyntaxCollector() =
    inherit VisualBasicSyntaxVisitor<EventStatementSyntax list>([])
    override __.VisitEventBlock (node, result) = (node.EventStatement)::result
    override __.VisitEventStatement(node, result) = node::result

type PropertyBlockSyntaxCollector() =
    inherit VisualBasicSyntaxVisitor<PropertyBlockSyntax list>([])
    override __.VisitPropertyBlock (node, result) = node::result

type PropertyStatementSyntaxCollector() =
    inherit VisualBasicSyntaxVisitor<PropertyStatementSyntax list>([])
    override __.VisitPropertyStatement (node, result) = node::result

type MethodSyntaxCollector() =
    inherit VisualBasicSyntaxVisitor<MethodStatementSyntax list>([])
    override __.VisitMethodBlock(node, result) = (node.BlockStatement :?> MethodStatementSyntax)::result
    override __.VisitMethodStatement(node, result) = node::result