/****************************************************************************
 * Copyright (c) 2017 xiaojun
 * Copyright (c) 2017 ~ 2021.1  liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

namespace GDFrameworkExtend.UIKit 
{
    using UnityEngine;

    public enum PanelState
    {
        Opening = 0,
        Hide = 1,
        Closed = 2,
    }
	
    /// <summary>
    /// IUIPanel.
    /// </summary>
    public partial interface IPanel
    {
        Transform Transform { get; }
		
        IPanelLoader Loader { get; set; }
		
        PanelInfo Info { get; set; }
		
        PanelState State { get; set; }

        void Init(IUIData uiData = null);

        void Open(IUIData uiData = null);

        void Show();

        void Hide();
		
        void Close(bool destroy = true);
    }
}