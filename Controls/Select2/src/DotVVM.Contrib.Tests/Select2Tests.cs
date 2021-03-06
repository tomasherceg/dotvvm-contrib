﻿using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Riganti.Utils.Testing.SeleniumCore;

namespace DotVVM.Contrib.Tests
{
    [TestClass]
    public class Select2Tests : SeleniumTestBase
    {
        [TestMethod]
        public void Select2_Sample1()
        {
            RunInAllBrowsers(browser =>
            {
                browser.NavigateToUrl("/Sample1");

                RunTestSubSection("List of strings", b =>
                {
                    PerformSelect2Test(b, b.ElementAt("fieldset", 0));
                });
                RunTestSubSection("List of objects", b =>
                {
                    PerformSelect2Test(b, b.ElementAt("fieldset", 1));
                });
            });
        }

        private static void PerformSelect2Test(BrowserWrapper browser, ElementWrapper fieldset)
        {
            var select2 = fieldset.Single(".select2");
            var input = select2.First("input");
            var result = fieldset.Single(".result");
            var addItemsButton = fieldset.ElementAt("button", 0);
            var changeSelectionButton = fieldset.ElementAt("button", 1);
            var submitButton = fieldset.ElementAt("button", 2);

            // verify the selection at the beginning
            select2.FindElements("li").ThrowIfDifferentCountThan(2);
            result.CheckIfTextEquals("Prague");

            // append tag
            input.SendKeys("Ne");
            input.SendKeys(Keys.Return);
            select2.FindElements("li").ThrowIfDifferentCountThan(3);

            // submit and check the selection on the server
            submitButton.Click().Wait();

            // verify the new tag appeared in the result
            result.CheckIfTextEquals("Prague,New York");

            // check the items offered in the list
            input.Click().Wait();
            browser.FindElements(".select2-container--open li.select2-results__option").ThrowIfDifferentCountThan(4);
            fieldset.Click();

            // add new items to the collection
            addItemsButton.Click().Wait();

            // check the items offered in the list were updated
            input.Click().Wait();
            browser.FindElements(".select2-container--open li.select2-results__option").ThrowIfDifferentCountThan(5);

            // select last item from the list
            browser.Last(".select2-container--open li.select2-results__option").Click();

            // submit and check the selection on the server
            submitButton.Click().Wait();
            result.CheckIfTextEquals("Prague,New York,Berlin");

            // replace the selection on server
            changeSelectionButton.Click().Wait();
            submitButton.Click().Wait();
            result.CheckIfTextEquals("New York,Paris");
        }

        [TestMethod]
        public void Select2_Sample2()
        {
            RunInAllBrowsers(browser =>
            {
                browser.NavigateToUrl("/Sample2");

                var select2 = browser.Single(".select2");
                var input = select2.First("input");
                var result = browser.Single(".number-of-requests");

                input.SendKeys("c");
                input.SendKeys(Keys.Return);

                browser.Wait();

                browser.FindElements(".select2-selection__choice").ThrowIfDifferentCountThan(1);
                Assert.AreEqual("1", result.GetInnerText());
            });
        }

        [TestMethod]
        public void Select2_Sample3()
        {
            RunInAllBrowsers(browser =>
            {
                browser.NavigateToUrl("/Sample3");

                ElementWrapper OpenSelectInput()
                {
                    browser.Single(".select2").Click();
                    return browser.First(".select2-search input");
                }
                var requestCount = browser.Single(".number-of-requests");
                var selectedValue = browser.Single(".selected-value");
                var displayValue = browser.Single(".select2-selection__rendered");

                var input = OpenSelectInput();
                input.SendKeys("c");
                input.SendKeys(Keys.Return);

                browser.Wait();

                Assert.AreEqual("c", displayValue.GetInnerText());
                Assert.AreEqual("2", selectedValue.GetInnerText());
                Assert.AreEqual("1", requestCount.GetInnerText());

                input = OpenSelectInput();
                input.SendKeys("a");
                input.SendKeys(Keys.Return);

                browser.Wait();

                Assert.AreEqual("a", displayValue.GetInnerText());
                Assert.AreEqual("0", selectedValue.GetInnerText());
                Assert.AreEqual("2", requestCount.GetInnerText());

                browser.Single("[data-ui=change-in-postback]").Click();
                Assert.AreEqual("c", displayValue.GetInnerText());
                Assert.AreEqual("2", selectedValue.GetInnerText());
                Assert.AreEqual("2", requestCount.GetInnerText());

                browser.Single("[data-ui=change-in-static-command]").Click();
                Assert.AreEqual("b", displayValue.GetInnerText());
                Assert.AreEqual("1", selectedValue.GetInnerText());
                Assert.AreEqual("2", requestCount.GetInnerText());
            });
        }
    }
}
