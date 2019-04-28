namespace Markify.CodeAnalyzer.Roslyn.Csharp

open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax

[<AbstractClass>]
type CSharpSyntaxVisitor<'a>(value : 'a) =
    inherit CSharpSyntaxVisitor()

    let mutable result : 'a = value

    member __.Visit(node) =
        base.Visit(node)
        result

    abstract member VisitFieldDeclaration : FieldDeclarationSyntax * 'a -> 'a
    default __.VisitFieldDeclaration (_, result) = result

    abstract member VisitEventDeclaration : EventDeclarationSyntax * 'a -> 'a
    default __.VisitEventDeclaration (_, result) = result

    abstract member VisitEventFieldDeclaration : EventFieldDeclarationSyntax * 'a -> 'a
    default __.VisitEventFieldDeclaration (_, result) = result

    abstract member VisitPropertyDeclaration : PropertyDeclarationSyntax * 'a -> 'a
    default __.VisitPropertyDeclaration (_, result) = result

    abstract member VisitMethodDeclaration : MethodDeclarationSyntax * 'a -> 'a
    default __.VisitMethodDeclaration (_, result) = result

    override this.VisitFieldDeclaration node =
        result <- this.VisitFieldDeclaration(node, result)
        base.VisitFieldDeclaration(node)

    override this.VisitEventDeclaration node =
        result <- this.VisitEventDeclaration(node, result)
        base.VisitEventDeclaration(node)

    override this.VisitEventFieldDeclaration node =
        result <- this.VisitEventFieldDeclaration(node, result)
        base.VisitEventFieldDeclaration(node)

    override this.VisitPropertyDeclaration node =
        result <- this.VisitPropertyDeclaration(node, result)
        base.VisitPropertyDeclaration(node)

    override this.VisitMethodDeclaration node =
        result <- this.VisitMethodDeclaration(node, result)
        base.VisitMethodDeclaration(node)

[<AbstractClass>]
type CSharpSyntaxWalker<'a>(value : 'a) =
    inherit CSharpSyntaxWalker()

    let mutable result : 'a = value

    member __.Search(node) = 
        base.Visit(node)
        result

    abstract member VisitClassDeclaration : ClassDeclarationSyntax * 'a -> 'a
    default __.VisitClassDeclaration(_, result) = result

    abstract member VisitStructDeclaration : StructDeclarationSyntax * 'a -> 'a
    default __.VisitStructDeclaration(_, result) = result

    abstract member VisitInterfaceDeclaration : InterfaceDeclarationSyntax * 'a -> 'a
    default __.VisitInterfaceDeclaration(_, result) = result

    abstract member VisitMethodDeclaration : MethodDeclarationSyntax * 'a -> 'a
    default __.VisitMethodDeclaration(_, result) = result

    abstract member VisitEnumDeclaration : EnumDeclarationSyntax * 'a -> 'a
    default __.VisitEnumDeclaration(_, result) = result

    abstract member VisitNamespaceDeclaration : NamespaceDeclarationSyntax * 'a -> 'a
    default __.VisitNamespaceDeclaration(_, result) = result

    abstract member VisitIdentifierName : IdentifierNameSyntax * 'a -> 'a
    default __.VisitIdentifierName(_, result) = result

    override this.VisitClassDeclaration node =
        result <- this.VisitClassDeclaration(node, result)
        base.VisitClassDeclaration(node)

    override this.VisitStructDeclaration node =
        result <- this.VisitStructDeclaration(node, result)
        base.VisitStructDeclaration(node)

    override this.VisitInterfaceDeclaration node =
        result <- this.VisitInterfaceDeclaration(node, result)
        base.VisitInterfaceDeclaration(node)

    override this.VisitMethodDeclaration node =
        result <- this.VisitMethodDeclaration(node, result)
        base.VisitMethodDeclaration(node)

    override this.VisitEnumDeclaration node =
        result <- this.VisitEnumDeclaration(node, result)
        base.VisitEnumDeclaration(node)

    override this.VisitNamespaceDeclaration node =
        result <- this.VisitNamespaceDeclaration(node, result)
        base.VisitNamespaceDeclaration(node)

    override this.VisitIdentifierName node =
        result <- this.VisitIdentifierName(node, result)
        base.VisitIdentifierName(node)

type NamespaceSyntaxCollector() =
    inherit CSharpSyntaxWalker<NamespaceDeclarationSyntax list>([])
    override __.VisitNamespaceDeclaration (node, result) = node::result

type ClassSyntaxCollector() =
    inherit CSharpSyntaxWalker<ClassDeclarationSyntax list>([])
    override __.VisitClassDeclaration (node, result) = node::result

type StructSyntaxCollector() =
    inherit CSharpSyntaxWalker<StructDeclarationSyntax list>([])   
    override __.VisitStructDeclaration (node, result) = node::result

type InterfaceSyntaxCollector() =
    inherit CSharpSyntaxWalker<InterfaceDeclarationSyntax list>([])  
    override __.VisitInterfaceDeclaration (node, result) = node::result

type EnumSyntaxCollector() =
    inherit CSharpSyntaxWalker<EnumDeclarationSyntax list>([])   
    override __.VisitEnumDeclaration (node, result) = node::result

type FieldSyntaxCollector() =
    inherit CSharpSyntaxVisitor<FieldDeclarationSyntax list>([])
    override __.VisitFieldDeclaration (node, result) = node::result

type EventSyntaxCollector() =
    inherit CSharpSyntaxVisitor<MemberDeclarationSyntax list>([])
    override __.VisitEventDeclaration (node, result) = (node :> MemberDeclarationSyntax)::result
    override __.VisitEventFieldDeclaration (node, result) = (node :> MemberDeclarationSyntax)::result

type PropertySyntaxCollector() =
    inherit CSharpSyntaxVisitor<PropertyDeclarationSyntax list>([])
    override __.VisitPropertyDeclaration (node, result) = node::result

type MethodSyntaxCollector() =
    inherit CSharpSyntaxVisitor<MethodDeclarationSyntax list>([])
    override __.VisitMethodDeclaration (node, result) = node::result