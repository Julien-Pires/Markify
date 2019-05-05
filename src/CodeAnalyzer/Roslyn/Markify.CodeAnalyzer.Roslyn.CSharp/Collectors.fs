namespace Markify.CodeAnalyzer.Roslyn.Csharp

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax

[<AbstractClass>]
type TypesCollector<'a>() =
    inherit CSharpSyntaxWalker()

    let mutable result : 'a list = []

    member __.Visit(node) =
        base.Visit(node)
        result

    member __.add = function
        | Some x -> x::result
        | None -> result

    abstract member VisitClassDeclaration : ClassDeclarationSyntax -> 'a option

    abstract member VisitStructDeclaration : StructDeclarationSyntax -> 'a option

    abstract member VisitInterfaceDeclaration : InterfaceDeclarationSyntax -> 'a option

    abstract member VisitDelegateDeclaration : DelegateDeclarationSyntax -> 'a option

    abstract member VisitEnumDeclaration : EnumDeclarationSyntax -> 'a option

    override this.VisitClassDeclaration node =
        result <- this.add <| this.VisitClassDeclaration(node)
        base.VisitClassDeclaration(node)

    override this.VisitStructDeclaration node =
        result <- this.add <| this.VisitStructDeclaration(node)
        base.VisitStructDeclaration(node)

    override this.VisitInterfaceDeclaration node =
        result <- this.add <| this.VisitInterfaceDeclaration(node)
        base.VisitInterfaceDeclaration(node)

    override this.VisitDelegateDeclaration node =
        result <- this.add <| this.VisitDelegateDeclaration(node)
        base.VisitDelegateDeclaration(node)

    override this.VisitEnumDeclaration node =
        result <- this.add <| this.VisitEnumDeclaration(node)
        base.VisitEnumDeclaration(node)

[<AbstractClass>]
type MembersCollector<'a>() =
    inherit CSharpSyntaxVisitor<'a option>()
    
    member this.Visit(node : TypeDeclarationSyntax) =
        node.Members
        |> Seq.choose this.Visit
        |> Seq.toList

type TypesCollector() =
    inherit TypesCollector<SyntaxNode>()
    override __.VisitClassDeclaration (node : ClassDeclarationSyntax) = Some (node :> SyntaxNode)
    override __.VisitStructDeclaration (node : StructDeclarationSyntax) = Some (node :> SyntaxNode)
    override __.VisitInterfaceDeclaration (node : InterfaceDeclarationSyntax) = Some (node :> SyntaxNode)
    override __.VisitEnumDeclaration (node : EnumDeclarationSyntax) = Some (node :> SyntaxNode)
    override __.VisitDelegateDeclaration (node : DelegateDeclarationSyntax) = Some (node :> SyntaxNode)

type FieldSyntaxCollector() =
    inherit MembersCollector<FieldDeclarationSyntax>()
    override __.VisitFieldDeclaration (node) = Some node

type EventSyntaxCollector() =
    inherit MembersCollector<MemberDeclarationSyntax>()
    override __.VisitEventDeclaration (node) = Some (node :> MemberDeclarationSyntax)
    override __.VisitEventFieldDeclaration (node) = Some (node :> MemberDeclarationSyntax)

type PropertySyntaxCollector() =
    inherit MembersCollector<PropertyDeclarationSyntax>()
    override __.VisitPropertyDeclaration (node) = Some node

type MethodSyntaxCollector() =
    inherit MembersCollector<MethodDeclarationSyntax>()
    override __.VisitMethodDeclaration (node) = Some node