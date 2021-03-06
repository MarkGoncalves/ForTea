#region License
//    Copyright 2012 Julien Lebosquain
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
#endregion
using GammaJul.ReSharper.ForTea.Parsing;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace GammaJul.ReSharper.ForTea.Tree {

	/// <summary>
	/// Represents a directive attribute, like <c>namespace="System"</c> in an import directive.
	/// </summary>
	internal sealed class T4DirectiveAttribute : T4CompositeElement, IT4DirectiveAttribute {

		/// <summary>
		/// Gets the role of a child node.
		/// </summary>
		/// <param name="nodeType">The type of the child node</param>
		protected override T4TokenRole GetChildRole(NodeType nodeType) {
			if (nodeType == T4TokenNodeTypes.Name)
				return T4TokenRole.Name;
			if (nodeType == T4TokenNodeTypes.Equal)
				return T4TokenRole.Separator;
			if (nodeType == T4TokenNodeTypes.Value)
				return T4TokenRole.Value;
			return T4TokenRole.Unknown;
		}

		/// <summary>
		/// Gets the node type of this element.
		/// </summary>
		public override NodeType NodeType {
			get { return T4ElementTypes.T4DirectiveAttribute; }
		}

		/// <summary>
		/// Gets the token representing the name of this node.
		/// </summary>
		/// <returns>The name token, or <c>null</c> if none is available.</returns>
		public IT4Token GetNameToken() {
			return (IT4Token) FindChildByRole((short) T4TokenRole.Name);
		}

		/// <summary>
		/// Gets the token representing the equal sign between the name and the value of this attribute.
		/// </summary>
		/// <returns>An equal token, or <c>null</c> if none is available.</returns>
		public IT4Token GetEqualToken() {
			return (IT4Token) FindChildByRole((short) T4TokenRole.Separator);
		}

		/// <summary>
		/// Gets the token representing the value of this attribute.
		/// </summary>
		/// <returns>A value token, or <c>null</c> if none is available.</returns>
		public IT4Token GetValueToken() {
			return (IT4Token) FindChildByRole((short) T4TokenRole.Value);
		}

		/// <summary>
		/// Gets the name of the node.
		/// </summary>
		/// <returns>The node name, or <c>null</c> if none is available.</returns>
		public string GetName() {
			IT4Token nameToken = GetNameToken();
			return nameToken != null ? nameToken.GetText() : null;
		}

		/// <summary>
		/// Gets the value of this attribute.
		/// </summary>
		/// <returns>The attribute value, or <c>null</c> if none is available.</returns>
		public string GetValue() {
			IT4Token valueToken = GetValueToken();
			return valueToken != null ? valueToken.GetText() : null;
		}

		/// <summary>
		/// Gets or sets the error associated with the value that have been identified at parsing time.
		/// </summary>
		public string ValueError { get; set; }

	}

}