using System.Linq;
using LanguageTranslator.ExtensionPoints;
using LanguageTranslator.Java;
using LanguageTranslator.Java.Interfaces;
using LanguageTranslator.Java.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LanguageTranslator
{
    public class StatementTranslator : CSharpSyntaxVisitor<IStmt>
    {
        private readonly SemanticModel semanticModel;
        private readonly IExtensionPoint[] extensionPoints;        

        public StatementTranslator(SemanticModel semanticModel, IExtensionPoint[] extensionPoints)
        {
            this.semanticModel = semanticModel;
            this.extensionPoints = extensionPoints;
        }

        public override IStmt VisitBlock(BlockSyntax node)
        {
            return new CompoundStmt
            {
                Statements = node.Statements.Select(Visit).ToArray()
            };
        }

        public override IStmt VisitIdentifierName(IdentifierNameSyntax node)
        {
            var result = extensionPoints.Translate(node, semanticModel, this);
            if (result != null)
                return result;
            var symbol = semanticModel.GetSymbolInfo(node).Symbol;
            if (!node.InImplicitThisContext(semanticModel))
            {
                return new IdentifierExpr
                {
                    Identifier = node.Identifier.ToString(),
                };
            }
            if (symbol.IsStatic)
            {
                return new MemberAccessExpr
                {
                    ObjectExpr = new IdentifierExpr { Identifier = symbol.ContainingType.Name },
                    MemberExpr = new IdentifierExpr { Identifier = node.Identifier.ToString() }
                };
            }
            return new MemberAccessExpr
            {
                ObjectExpr = new ThisExpr(),
                MemberExpr = new IdentifierExpr { Identifier = node.Identifier.ToString() }
            };
        }

        public override IStmt VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            return new LiteralExpr
            {
                Value = node.GetText().ToString()
            };
        }

        public override IStmt VisitArrayCreationExpression(ArrayCreationExpressionSyntax node)
        {
            var result = extensionPoints.Translate(node, semanticModel, this);
            if (result != null)
                return result;
            if (node.Initializer != null)
            {
                return new IndexExpr { Elements = node.Initializer.Expressions.Select(Visit).ToArray() };
            }
            //вот тут подумать про [][] и [,]
            var mainRank = Visit(node.Type.RankSpecifiers[0].Sizes[0]); 
            return new ArrayCreationExpr { Rank = mainRank };
        }

        public override IStmt VisitCastExpression(CastExpressionSyntax node)
        {
            //Посмотреть, как кастуется
            return Visit(node.Expression);
        }

        public override IStmt VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            var result = extensionPoints.Translate(node, semanticModel, this);
            if (result != null)
                return result;
            return new CallExpr
            {
                Expression = Visit(node.Expression),
                Arguments = node.ArgumentList.Arguments.Select(Visit).ToArray()
            };
        }

        public override IStmt VisitArgument(ArgumentSyntax node)
        {
            return Visit(node.Expression);
        }

        public override IStmt VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            var result = extensionPoints.Translate(node, semanticModel, this);
            if (result != null)
                return result;
            return new MemberAccessExpr
            {
                ObjectExpr = Visit(node.Expression),
                MemberExpr = new IdentifierExpr { Identifier = node.Name.Identifier.ToString() }
            };
        }

        public override IStmt VisitReturnStatement(ReturnStatementSyntax node)
        {
            if (node.Expression == null)
                return new ReturnStmt();
            return new ReturnStmt
            {
                Expr = Visit(node.Expression)
            };
        }

        public override IStmt VisitIfStatement(IfStatementSyntax node)
        {
            return new IfElseStmt
            {
                Condition = Visit(node.Condition),
                ThenBody = WrapToBlock(Visit(node.Statement)),
                ElseBody = node.Else != null ? WrapToBlock(Visit(node.Else)) : null
            };
        }

        public override IStmt VisitElseClause(ElseClauseSyntax node)
        {
            return Visit(node.Statement);
        }

        public override IStmt VisitThisExpression(ThisExpressionSyntax node)
        {
            return new ThisExpr();
        }

        public override IStmt VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
        {
            return new ParenExpr
            {
                Expression = Visit(node.Expression)
            };
        }

        public override IStmt VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            return new CompoundLocalDeclStmt
            {
                Declarations = node.DescendantNodes().OfType<VariableDeclaratorSyntax>().Select(Visit).ToArray()
            };
        }

        public override IStmt VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            return new CompoundLocalDeclStmt
            {
                Declarations = node.DescendantNodes().OfType<VariableDeclaratorSyntax>().Select(Visit).ToArray()
            };
        }

        public override IStmt VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            var result = extensionPoints.Translate(node, semanticModel, this);
            if (result != null)
                return result;
            return new BinaryExpr
            {
                Left = Visit(node.Left),
                Operation = node.OperatorToken.ValueText,
                Right = Visit(node.Right)
            };
        }

        public override IStmt VisitElementAccessExpression(ElementAccessExpressionSyntax node)
        {
            //посмотреть ElementAccess
            return new ReturnStmt();
        }

        public override IStmt VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            return new UnaryExpr
            {
                IsPrefixExpr = true,
                Expression = Visit(node.Operand),
                Operation = node.OperatorToken.Text
            };
        }

        public override IStmt VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            return new UnaryExpr
            {
                IsPrefixExpr = false,
                Expression = Visit(node.Operand),
                Operation = node.OperatorToken.Text
            };
        }

        public override IStmt VisitWhileStatement(WhileStatementSyntax node)
        {
            return new WhileStmt
            {
                Condition = Visit(node.Condition),
                Body = WrapToBlock(Visit(node.Statement))
            };
        }

        public override IStmt VisitDoStatement(DoStatementSyntax node)
        {
            return new DoWhileStmt
            {
                Condition = Visit(node.Condition),
                Body = WrapToBlock(Visit(node.Statement))
            };
        }

        public override IStmt VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            return Visit(node.Expression);
        }

        public override IStmt VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            var result = extensionPoints.Translate(node, semanticModel, this);
            if (result != null)
                return result;
            return new BinaryExpr
            {
                Left = Visit(node.Left),
                Operation = node.OperatorToken.Text,
                Right = Visit(node.Right)
            };
        }

        public override IStmt VisitEqualsValueClause(EqualsValueClauseSyntax node)
        {
            return Visit(node.Value);
        }

        public override IStmt VisitImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax node)
        {
            return new IndexExpr
            {
                Elements = node.Initializer.Expressions.Select(Visit).ToArray()
            };
        }

        public override IStmt VisitConditionalExpression(ConditionalExpressionSyntax node)
        {
            return new ConditionalExpr
            {
                Condition = Visit(node.Condition),
                ThenStmt = Visit(node.WhenTrue),
                ElseStmt = Visit(node.WhenFalse)
            };
        }

        public override IStmt VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            var symbol = semanticModel.GetDeclaredSymbol(node);
            var customRuleResult = extensionPoints.Translate(node, semanticModel, this);
            if (customRuleResult != null)
                return customRuleResult;
            return new LocalDeclStmt
            {
                DeclName = node.Identifier.ToString(),
                Initialization = Visit(node.Initializer),
                TypeSymbol = SymbolHelper.GetVariableSymbol(symbol)
            };
        }

        public override IStmt VisitPredefinedType(PredefinedTypeSyntax node)
        {
            return new IdentifierExpr
            {
                Identifier = node.Keyword.Text
            };
        }

        public override IStmt VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            var result = extensionPoints.Translate(node, semanticModel, this);
            if (result != null)
                return result;
            //new A { a = 4, b = 5 } --> ??
            //...
            //new A(4, 5) --> new A(4,5);
            return new ObjectCreationExpr
            {
                TypeInformation = new TypeInformation
                {
                    TypeName = node.Type.ToString()
                },
                Arguments = node.ArgumentList != null ? node.ArgumentList.Arguments.Select(Visit).ToArray() : new IStmt[0]
            };
        }

        public override IStmt VisitSwitchStatement(SwitchStatementSyntax node)
        {
            var switchCases = node.ChildNodes().OfType<SwitchSectionSyntax>().Select(Visit).Cast<SwitchCaseStmt>().ToArray();
            return new SwitchStmt
            {
                Condition = Visit(node.Expression),
                Cases = switchCases
            };
        }

        public override IStmt VisitBreakStatement(BreakStatementSyntax node)
        {
            return new BreakStmt();
        }

        public override IStmt VisitSwitchSection(SwitchSectionSyntax node)
        {
            var labels = node.ChildNodes().OfType<CaseSwitchLabelSyntax>().Select(label => label.Value != null ? Visit(label.Value) : null).ToArray();
            return new SwitchCaseStmt
            {
                Labels = labels,
                Statements = node.Statements.Select(Visit).ToArray()
            };
        }

        public override IStmt VisitForStatement(ForStatementSyntax node)
        {
            return new ForStmt
            {
                Initializers = node.Initializers.Select(Visit).ToArray(),
                Condition = Visit(node.Condition),
                Incrementors = node.Incrementors.Select(Visit).ToArray(),
                Body = WrapToBlock(Visit(node.Statement))
            };
        }

        public override IStmt VisitForEachStatement(ForEachStatementSyntax node)
        {
            //foreach(var a : b) -> ??
            return new ReturnStmt();
        }

        public override IStmt VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            //посмотреть лямбды в java
            return new ReturnStmt();
        }

        public override IStmt VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
            //посмотреть лямбды в java
            return new ReturnStmt();
        }

        public override IStmt VisitParameter(ParameterSyntax node)
        {
            return new IdentifierExpr
            {
                Identifier = node.Identifier.ToString()
            };
        }

        private static IStmt WrapToBlock(IStmt node)
        {
            return node is CompoundStmt
                ? node
                : new CompoundStmt
                {
                    Statements = new[] { node }
                };
        }
    }
}
